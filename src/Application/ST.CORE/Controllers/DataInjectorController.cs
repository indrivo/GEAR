using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;

namespace ST.CORE.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class DataInjectorController : ControllerBase
	{
		/// <summary>
		/// Inject dynamic service
		/// </summary>
		private readonly IDynamicService _dynamicService;

		/// <summary>
		/// Inject context
		/// </summary>
		private readonly EntitiesDbContext _context;

		public DataInjectorController(IDynamicService dynamicService, EntitiesDbContext context)
		{
			_dynamicService = dynamicService;
			_context = context;
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
				var parsed = JsonConvert.DeserializeObject(obj, _dynamicService.Table(tableName).Object.GetType());
				var rq = await _dynamicService.Table(tableName).Add(parsed);
				return new JsonResult(rq);
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
				var parsed = JsonConvert.DeserializeObject(obj, _dynamicService.Table(tableName).Object.GetType());
				var rq = await _dynamicService.Table(tableName).Update(parsed);
				return new JsonResult(rq);
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
			var rq = await _dynamicService.Table(tableName).GetById<dynamic>(id);
			return new JsonResult(rq);
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
			var rq = await _dynamicService.Table(tableName).GetAll<dynamic>();
			return new JsonResult(rq);
		}

		/// <summary>
		/// Get all where
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="filters"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetAllWhere(string tableName, IEnumerable<Filter> filters = null)
		{
			var (isValid, errors) = await IsValid(tableName);
			if (!isValid) return new JsonResult(errors);
			var rq = await _dynamicService.Table(tableName).GetAll<dynamic>(null, filters);
			return new JsonResult(rq);
		}
	}
}