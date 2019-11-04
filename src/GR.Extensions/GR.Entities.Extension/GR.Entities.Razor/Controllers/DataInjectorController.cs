using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Security.Abstractions.Attributes;
using GR.Entities.Security.Abstractions.Enums;
using GR.Entities.Security.Abstractions.Helpers;

namespace GR.Entities.Razor.Controllers
{
    [Authorize, Route("api/[controller]/[action]")]
    public class DataInjectorController : Controller
    {
        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IDynamicService _dynamicService;

        /// <summary>
        /// json settings
        /// </summary>
        private readonly JsonSerializerSettings _jsonSerializeOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dynamicService"></param>
        public DataInjectorController(IDynamicService dynamicService)
        {
            _dynamicService = dynamicService;
            _jsonSerializeOptions = new JsonSerializerSettings
            {
                DateFormatString = GearSettings.Date.DateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Get item by id async
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeEntity(EntityAccessType.Read)]
        public async Task<JsonResult> GetByIdWithIncludesAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.Update)]
        public async Task<JsonResult> UpdateAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.Write)]
        public async Task<JsonResult> AddAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.Write)]
        public async Task<JsonResult> AddRangeAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.DeletePermanent)]
        public async Task<JsonResult> DeletePermanentWhereAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.Delete, EntityAccessType.Update)]
        public async Task<JsonResult> DeleteWhereAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.Read)]
        public async Task<JsonResult> GetAllWhereNoIncludesAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
        [AuthorizeEntity(EntityAccessType.Read)]
        public async Task<JsonResult> CountAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));
            var f = filters.ToDictionary(x => x.Parameter, y => y.Value);
            var rq = await _dynamicService.Table(data.EntityName).Count(f);
            return Json(rq);
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeEntity(EntityAccessType.Read)]
        public async Task<JsonResult> CountAllAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
            var serial = JsonConvert.SerializeObject(data.Filters);
            var filters = ParseFilters(serial).ToList();
            var f = filters.ToDictionary(x => x.Parameter, y => y.Value);
            var rq = await _dynamicService.Table(data.EntityName).Count(f);
            return Json(rq);
        }

        /// <summary>
        /// Get all with includes
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeEntity(EntityAccessType.Read)]
        public async Task<JsonResult> GetAllWhereWithIncludesAsync([Required][FromBody] RequestData data)
        {
            if (data == null) return RequestData.InvalidRequest;
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
                foreach (var filter in f.Where(filter => filter.Value != null).Where(filter => filter.Value.ToString().IsGuid()))
                {
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
    }
}