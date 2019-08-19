using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Data;
using ST.Entities.Security.Abstractions;
using ST.Entities.Security.Abstractions.Enums;

namespace ST.Entities.Razor.Controllers
{
    [Authorize, Route("api/[controller]/[action]")]
    public class DataInjectorController : Controller
    {
        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IDynamicService _dynamicService;

        /// <summary>
        /// Inject entity role access manager
        /// </summary>
        private readonly IEntityRoleAccessManager _accessManager;

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly EntitiesDbContext _context;

        /// <summary>
        /// json settings
        /// </summary>
        private readonly JsonSerializerSettings _jsonSerializeOptions;

        /// <summary>
        /// Access denied
        /// </summary>
        private ResultModel AccessDenied => new ResultModel
        {
            Errors = new List<IErrorModel>
            {
                new ErrorModel(nameof(Settings.ACCESS_DENIED_MESSAGE), Settings.ACCESS_DENIED_MESSAGE)
            }
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dynamicService"></param>
        /// <param name="context"></param>
        /// <param name="accessManager"></param>
        public DataInjectorController(IDynamicService dynamicService, EntitiesDbContext context, IEntityRoleAccessManager accessManager)
        {
            _dynamicService = dynamicService;
            _context = context;
            _accessManager = accessManager;
            _jsonSerializeOptions = new JsonSerializerSettings
            {
                DateFormatString = Settings.Date.DateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Is valid entity
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<ResultModel<TableModel>> IsValid(string tableName)
        {
            var badParams = new ErrorModel(string.Empty, "Entity not identified!");
            var entityNotFound = new ErrorModel(string.Empty, "Entity not found!");
            var result = new ResultModel<TableModel>();
            if (string.IsNullOrEmpty(tableName))
            {
                result.Errors.Add(badParams);
                return result;
            }
            var entity = await _context.Table.FirstOrDefaultAsync(x => x.Name == tableName);

            if (entity == null)
            {
                result.Errors.Add(entityNotFound);
                return result;
            }

            result.IsSuccess = true;
            result.Result = entity;
            return result;
        }

        /// <summary>
        /// Get item by id async
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetByIdWithIncludesAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveReadAccessAsync(isValid.Result.Id);
            if (!grant) return Json(AccessDenied);
            Guid.TryParse(data.Object, out var itemId);
            var rq = await _dynamicService.GetByIdWithInclude(data.EntityName, itemId);
            return Json(rq, _jsonSerializeOptions);
        }

        /// <summary>
        /// Update item in database
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id, EntityAccessType.Update);
            if (!grant) return Json(AccessDenied);
            var result = new ResultModel();
            var operationalTable = _dynamicService.Table(data.EntityName);
            try
            {
                var parsed = JsonConvert.DeserializeObject(data.Object, operationalTable.Type, _jsonSerializeOptions);
                var rq = await operationalTable.Update(parsed);
                return Json(rq);
            }
            catch (JsonSerializationException e)
            {
                result.Errors.Add(new ErrorModel(nameof(JsonSerializationException), e.Message));
            }
            catch (Exception e)
            {
                result.Errors.Add(new ErrorModel(nameof(Exception), e.ToString()));
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Add new object to entity
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id, EntityAccessType.Write);
            if (!grant) return Json(AccessDenied);
            var result = new ResultModel();
            try
            {
                var parsed = JsonConvert.DeserializeObject(data.Object, _dynamicService.Table(data.EntityName).Type, _jsonSerializeOptions);
                var rq = await _dynamicService.Table(data.EntityName).Add(parsed);
                return Json(rq);
            }
            catch (JsonReaderException e)
            {
                result.Errors.Add(new ErrorModel(nameof(JsonReaderException), e.Message));
            }
            catch (Exception e)
            {
                result.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
            }
            return new JsonResult(result);
        }


        /// <summary>
        /// Add new object list to entity
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddRangeAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id, EntityAccessType.Write);
            if (!grant) return Json(AccessDenied);
            var result = new ResultModel();
            try
            {
                var tableManger = _dynamicService.Table(data.EntityName);
                var list = typeof(List<>);
                var listOfType = list.MakeGenericType(tableManger.Type);
                var parsed = JsonConvert.DeserializeObject(data.Object, listOfType, _jsonSerializeOptions);
                var rq = await _dynamicService.Table(data.EntityName).AddRange(parsed as IEnumerable<object>);
                return Json(rq);
            }
            catch (JsonReaderException e)
            {
               result.Errors.Add(new ErrorModel(nameof(JsonReaderException), e.Message));
            }
            catch (Exception e)
            {
                result.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
            }

            return Json(result);
        }


        /// <summary>
        /// Delete permanent by filters
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<JsonResult> DeletePermanentWhereAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id, EntityAccessType.DeletePermanent);
            if (!grant) return Json(AccessDenied);
            var result = new ResultModel();
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));
            var rqGet = await _dynamicService.Table(data.EntityName).GetAll<dynamic>(null, filters);
            if (rqGet.IsSuccess)
            {
                var taskResults = rqGet.Result.Select(async item =>
                    await _dynamicService.Table(data.EntityName).DeletePermanent<object>((Guid)item.Id)).Select(x => x.Result);
                result.IsSuccess = true;
                result.Result = taskResults;
                return Json(result);
            }

            result.Errors.Add(new ErrorModel(nameof(EmptyResult), "No item to delete!"));
            return Json(result);
        }


        /// <summary>
        /// Delete where by filters
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<JsonResult> DeleteWhereAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id, EntityAccessType.Delete);
            if (!grant) return Json(AccessDenied);
            var result = new ResultModel();
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));
            var rqGet = await _dynamicService.Table(data.EntityName).GetAll<dynamic>(null, filters);
            if (rqGet.IsSuccess)
            {
                var taskResults = rqGet.Result.Select(async item =>
                    await _dynamicService.Table(data.EntityName).Delete<object>((Guid)item.Id)).Select(x => x.Result);
                result.IsSuccess = true;
                result.Result = taskResults;
                return Json(result);
            }

            result.IsSuccess = false;
            result.Errors.Add(new ErrorModel(nameof(EmptyResult), "No item to delete!"));
            return Json(result);
        }


        /// <summary>
        /// Get all with no includes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetAllWhereNoIncludesAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveReadAccessAsync(isValid.Result.Id);
            if (!grant) return Json(AccessDenied);
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));
            var rq = await _dynamicService.Table(data.EntityName).GetAll<dynamic>(null, filters);
            return Json(rq);
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> CountAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id);
            if (!grant) return Json(AccessDenied);
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));
            var f = filters.ToDictionary(x => x.Parameter, y => y.Value);
            var rq = await _dynamicService.Table(data.EntityName).Count(f);
            return Json(rq);
        }


        /// <summary>
        /// Get all with includes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetAllWhereWithIncludesAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var isValid = await IsValid(data.EntityName);
            if (!isValid.IsSuccess) return new JsonResult(isValid);
            var grant = await _accessManager.HaveAccessAsync(isValid.Result.Id);
            if (!grant) return Json(AccessDenied);
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));
            var rq = await _dynamicService.Table(data.EntityName).GetAllWithInclude<dynamic>(null, filters);
            return Json(rq);
        }

        /// <summary>
        /// Parse filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        private static IEnumerable<Filter> ParseFilters(string filters)
        {
            if (string.IsNullOrEmpty(filters)) return null;
            try
            {
                var f = JsonConvert.DeserializeObject<IEnumerable<Filter>>(filters).ToList();
                foreach (var filter in f)
                {
                    if (filter.Value == null) continue;
                    if (!filter.Value.ToString().IsGuid()) continue;
                    Guid.TryParse(filter.Value?.ToString(), out var val);
                    filter.Value = val;
                }
                return f;
            }
            catch
            {
                return null;
            }
        }

        public class RequestData
        {
            public string EntityName { get; set; }
            public IEnumerable<Filter> Filters { get; set; } = new List<Filter>();
            public string Object { get; set; }
            public static JsonResult InvalidRequest => new JsonResult(new ResultModel
            {
                Errors = new List<IErrorModel>
                {
                    new ErrorModel("invalid_data", "Invalid data!")
                }
            });
        }
    }
}