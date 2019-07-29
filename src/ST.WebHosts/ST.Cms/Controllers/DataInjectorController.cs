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
using ST.Entities.Data;

namespace ST.Cms.Controllers
{
	[Route("api/[controller]/[action]")]
	[Authorize]
	public class DataInjectorController : Controller
	{
		/// <summary>
		/// Inject dynamic service
		/// </summary>
		private readonly IDynamicService _dynamicService;

		/// <summary>
		/// Inject context
		/// </summary>
		private readonly EntitiesDbContext _context;

		/// <summary>
		/// json settings
		/// </summary>
		private readonly JsonSerializerSettings _jsonSerializeOptions;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dynamicService"></param>
		/// <param name="context"></param>
		public DataInjectorController(IDynamicService dynamicService, EntitiesDbContext context)
		{
			_dynamicService = dynamicService;
			_context = context;
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
		private async Task<(bool, ResultModel)> IsValid(string tableName)
		{
			if (string.IsNullOrEmpty(tableName)) return (false, new ResultModel
			{
				Errors = new List<IErrorModel> { new ErrorModel(string.Empty, "Entity not identified!") }
			});

			var entity = await _context.Table.FirstOrDefaultAsync(x => x.Name == tableName);

			if (entity == null) return (false, new ResultModel
			{
				Errors = new List<IErrorModel> { new ErrorModel(string.Empty, "Entity not found!") }
			});

			return (true, default);
		}

		#region Old way to manipulate with data, but need to find where is used

		/// <summary>
		/// Add new object to entity
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> Add(string tableName, string obj)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			try
			{
				var parsed = JsonConvert.DeserializeObject(obj, _dynamicService.Table(tableName).Type, _jsonSerializeOptions);
				var rq = await _dynamicService.Table(tableName).Add(parsed);
				return Json(rq);
			}
			catch (Exception e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.ToString()) }
				});
			}
		}

		/// <summary>
		/// Update item in database
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> Update(string tableName, string obj)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			try
			{
				var parsed = JsonConvert.DeserializeObject(obj, _dynamicService.Table(tableName).Type, _jsonSerializeOptions);
				var rq = await _dynamicService.Table(tableName).Update(parsed);
				return Json(rq);
			}
			catch (Exception e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.ToString()) }
				});
			}
		}

		/// <summary>
		/// Get item by id
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetById(string tableName, Guid id)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			var rq = await _dynamicService.GetById(tableName, id);
			return Json(rq);
		}

		/// <summary>
		/// Get item by id
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetByIdWithInclude(string tableName, Guid id)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			var rq = await _dynamicService.GetByIdWithInclude(tableName, id);
			return Json(rq);
		}

		/// <summary>
		/// Get all items
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetAll(string tableName)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			var rq = await _dynamicService.GetAll(tableName);
			return Json(rq);
		}

		/// <summary>
		/// Get all items
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetAllWithInclude(string tableName)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			var rq = await _dynamicService.GetAllWithIncludeAsDictionaryAsync(tableName);
			return Json(rq);
		}

		/// <summary>
		/// Get all where
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="filters"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetAllWhere(string tableName, string filters)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			var f = ParseFilters(filters);
			var rq = await _dynamicService.Table(tableName).GetAllWithInclude<dynamic>(null, f);
			return Json(rq);
		}

		#endregion

		#region Async methods

		/// <summary>
		/// Get item by id async
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> GetByIdWithIncludesAsync([Required][FromBody] RequestData data)
		{
			if (data == null) return RequestData.InvalidRequest;
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
			try
			{
				var parsed = JsonConvert.DeserializeObject(data.Object, _dynamicService.Table(data.EntityName).Type,
					_jsonSerializeOptions);
				var rq = await _dynamicService.Table(data.EntityName).Update(parsed);
				return Json(rq);
			}
			catch (JsonSerializationException e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.Message) }
				});
			}
			catch (Exception e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.ToString()) }
				});
			}
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
			try
			{
				var parsed = JsonConvert.DeserializeObject(data.Object, _dynamicService.Table(data.EntityName).Type, _jsonSerializeOptions);
				var rq = await _dynamicService.Table(data.EntityName).Add(parsed);
				return Json(rq);
			}
			catch (JsonReaderException e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.Message) }
				});
			}
			catch (Exception e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.Message) }
				});
			}
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.Message) }
				});
			}
			catch (Exception e)
			{
				return new JsonResult(new ResultModel
				{
					Errors = new List<IErrorModel> { new ErrorModel(string.Empty, e.Message) }
				});
			}
		}


		/// <summary>
		/// Delete permanent by filters
		/// </summary>
		/// <returns></returns>
		[HttpDelete]
		public async Task<JsonResult> DeletePermanentWhereAsync([Required][FromBody] RequestData data)
		{
			if (data == null) return RequestData.InvalidRequest;
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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

			result.IsSuccess = false;
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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
			var (isValid, errors) = await IsValid(data.EntityName);
			if (!isValid) return new JsonResult(errors);
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

		#endregion
	}
}