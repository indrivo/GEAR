using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ST.BaseBusinessRepository;
using ST.CORE.Models;
using ST.Entities.Constants;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Tables;
using ST.Entities.Services;
using ST.Entities.Services.Abstraction;
using ST.Entities.Utils;
using ST.Entities.ViewModels.Table;
using ST.Identity.Attributes;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;

namespace ST.CORE.Controllers.Entity
{
	/// <inheritdoc />
	/// <summary />
	/// <summary>
	/// Forms manipulation
	/// </summary>
	[Authorize]
	public class TableController : Controller
	{
		/// <summary>
		/// Inject logger
		/// </summary>
		private readonly ILogger<TableController> _logger;

		/// <summary>
		/// Inject App Context
		/// </summary>
		private readonly ApplicationDbContext _context;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="contextApp"></param>
		/// <param name="repository"></param>
		/// <param name="configuration"></param>
		/// <param name="context"></param>
		/// <param name="env"></param>
		/// <param name="logger"></param>
		/// <param name="appContext"></param>
		public TableController(ApplicationDbContext contextApp, IBaseBusinessRepository<EntitiesDbContext> repository,
			IConfiguration configuration, EntitiesDbContext context, IHostingEnvironment env,
			ILogger<TableController> logger, ApplicationDbContext appContext)
		{
			ContextApp = contextApp;
			Configuration = configuration;
			Repository = repository;
			Context = context;
			_logger = logger;
			_context = appContext;
			ConnectionString = Entities.Utils.ConnectionString.Get(configuration, env);
		}

		private static IConfiguration Configuration { get; set; }

		private IBaseBusinessRepository<EntitiesDbContext> Repository { get; }

		private EntitiesDbContext Context { get; }

		private ApplicationDbContext ContextApp { get; }

		private (DbProviderType, string) ConnectionString { get; set; }

		/// <summary>
		/// Create table
		/// </summary>
		/// <returns></returns>

		// GET: Users/Create
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableCreate)]
		public IActionResult Create()
		{
			var model = new CreateTableViewModel
			{
				EntityTypes = Repository.GetAll<EntityType>(x => x.IsDeleted == false)
			};
			return View(model);
		}

		/// <summary>
		/// Get table service
		/// </summary>
		/// <returns></returns>
		[NonAction]
		private ITablesService GetSqlService()
		{
			return ConnectionString.Item1.Equals(DbProviderType.MsSqlServer) ?
															new TablesService(Repository)
															: ConnectionString.Item1.Equals(DbProviderType.PostgreSql)
															? new NpgTablesService(Repository)
															: null;
		}

		/// <summary>
		/// Create a table
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableCreate)]
		public async Task<IActionResult> Create(CreateTableViewModel model)
		{
			if (ModelState.IsValid)
			{
				var entityType =
					await Context.EntityTypes.FirstOrDefaultAsync(x => x.Id == Guid.Parse(model.SelectedTypeId));
				if (entityType != null)
				{
					var m = new CreateTableViewModel
					{
						Name = model.Name,
						EntityType = entityType.Name,
						Description = model.Description
					};
					var table = Repository.Add<TableModel, CreateTableViewModel>(m);

					if (table.IsSuccess)
					{
						var resultModel = await Repository
							.GetAllIncluding<TableModel>(x => x.Include(s => s.TableFields), x => x.Id == table.Result)
							.AsNoTracking().FirstOrDefaultAsync();
						ITablesService sqlService = GetSqlService();
						var response = sqlService.CreateSqlTable(resultModel, ConnectionString.Item2);
						if (response.Result)
							return RedirectToAction("Edit", "Table", new { id = table.Result, tab = "one" });

						return View(model);
					}
				}

				return View(model);
			}
			else
			{
				return View(model);
			}
		}

		/// <summary>
		/// List with entity
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableRead)]
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// List of tables
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult OrderList(DTParameters param)
		{
			var filtered = Context.Filter<TableModel>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount);


			var orderList = filtered.Select(o => new TableModel
			{
				Id = o.Id,
				Name = o.Name,
				IsDeleted = o.IsDeleted,
				Author = o.Author,
				Created = o.Created,
				ModifiedBy = o.ModifiedBy,
				Description = o.Description,
				EntityType = o.EntityType,
				Changed = o.Changed,
				IsSystem = o.IsSystem,
				TableFields = o.TableFields
			});

			var finalResult = new DTResult<TableModel>
			{
				draw = param.Draw,
				data = orderList.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count
			};

			return Json(finalResult);
		}

		/// <param name="id"></param>
		/// <param name="tab"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableUpdate)]
		public async Task<IActionResult> Edit(Guid id, string tab)
		{
			var model = Repository.GetById<TableModel, UpdateTableViewModel>(id);
			if (model.IsSuccess)
			{
				model.Result.TableFields = Repository.GetAll<TableModelFields>(x => x.TableId == id);
				if (model.Result.ModifiedBy != null)
					model.Result.ModifiedBy = ContextApp.Users
						.SingleOrDefaultAsync(m => m.Id == model.Result.ModifiedBy).Result.NormalizedUserName.ToLower();
				model.Result.Groups = await Context.TableFieldGroups.Include(s => s.TableFieldTypes).ToListAsync();
				model.Result.TabName = tab;
				return View(model.Result);
			}
			else
			{
				return RedirectToAction("Index", "Table", new { page = 1, perPage = 10 });
			}
		}

		/// <summary>
		/// Update table fields
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableUpdate)]
		public IActionResult Edit(UpdateTableViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var table = Repository.Refresh<TableModel, UpdateTableViewModel>(model);

			if (table.IsSuccess)
			{
				return RedirectToAction(nameof(Index), "Table");
			}

			ModelState.AddModelError(string.Empty, "Something went wrong on server");
			return View(model);
		}

		/// <summary>
		/// Change status is deleted in true
		/// </summary>
		/// <param name="tableId"></param>
		/// <returns></returns>
		[HttpDelete]
		public JsonResult RemoveTableModel(Guid tableId)
		{
			var response = Repository.RemoveByParams<TableModel>(s => s.Id == tableId);
			Repository.RemoveByParams<TableModelFields>(s => s.TableId == tableId);
			return Json(response);
		}

		/// <summary>
		/// Get view for add fields
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddField(Guid id, string type)
		{
			var entitiesList = await Repository.GetAll<TableModel>().ToListAsync();
			var fieldType = await Context.TableFieldTypes.FirstOrDefaultAsync(x => x.Name == type.Trim());
			var fieldTypeConfig = Context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id);
			var configurations = new List<FieldConfigViewModel>();
			foreach (var item in fieldTypeConfig)
			{
				configurations.Add(new FieldConfigViewModel
				{
					Name = item.Name,
					Type = item.Type,
					ConfigId = item.Id,
					Description = item.Description
				});
			}

			var selectList = new List<string>();
			foreach (var item in entitiesList)
			{
				//selectList.Add(item.EntityType + "." + item.Name);
				selectList.Add(item.Name);
			}

			var model = new CreateTableFieldViewModel
			{
				TableId = id,
				Configurations = configurations,
				TableFieldTypeId = fieldType.Id,
				DataType = fieldType.DataType,
				Parameter = type,
				EntitiesList = selectList
			};

			return View(model);
		}

		/// <summary>
		/// Create a field
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddField(CreateTableFieldViewModel field)
		{
			var tableName = Context.Table.FirstOrDefault(x => x.Id == field.TableId)?.Name;
			var schema = Context.Table.FirstOrDefault(x => x.Id == field.TableId)?.EntityType;
			if (tableName == null) return View();
			ITablesService sqlService = GetSqlService();
			field = field.CreateSqlField(ConnectionString);
			var insertField = sqlService.AddFieldSql(field, tableName, ConnectionString.Item2, true, schema);
			// Save field model in the dataBase
			if (!insertField.Result) return View();
			var configValues = new List<TableFieldConfigValues>();
			var model = new TableModelFields
			{
				DataType = field.DataType,
				TableId = field.TableId,
				Description = field.Description,
				Name = field.Name,
				DisplayName = field.DisplayName,
				AllowNull = field.AllowNull,
				Synchronized = true,
				TableFieldTypeId = field.TableFieldTypeId,
			};
			foreach (var item in field.Configurations)
			{
				configValues.Add(new TableFieldConfigValues
				{
					TableFieldConfigId = item.ConfigId,
					TableModelFieldId = model.Id,
					Value = item.Value,
				});
			}

			model.TableFieldConfigValues = configValues;
			var table = Repository.Add<TableModelFields, TableModelFields>(model);
			return RedirectToAction("Edit", "Table", new { id = field.TableId, tab = "two" });
		}

		/// <summary>
		/// Get view for add fields
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fieldId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> EditField(Guid fieldId, Guid type)
		{
			var fieldType = await Context.TableFieldTypes.FirstOrDefaultAsync(x => x.Id == type);
			var fieldTypeConfig = Context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id);
			// Clean Code
			var field = await Context.TableFields.FirstOrDefaultAsync(x => x.Id == fieldId);
			var configFields = Context.TableFieldConfigValues.Where(x => x.TableModelFieldId == fieldId);
			var configurations = new List<FieldConfigViewModel>();
			foreach (var item in configFields)
			{
				configurations.Add(new FieldConfigViewModel
				{
					Name = fieldTypeConfig.Single(x => x.Id == item.TableFieldConfigId).Name,
					Type = fieldTypeConfig.Single(x => x.Id == item.TableFieldConfigId).Type,
					ConfigId = item.TableFieldConfigId,
					Description = fieldTypeConfig.Single(x => x.Id == item.TableFieldConfigId).Description,
					Value = item.Value
				});
			}

			var model = new CreateTableFieldViewModel
			{
				Id = fieldId,
				TableId = field.TableId,
				Configurations = configurations,
				TableFieldTypeId = field.TableFieldTypeId,
				DataType = field.DataType,
				Parameter = fieldType.Name,
				Name = field.Name,
				DisplayName = field.DisplayName,
				AllowNull = field.AllowNull,
				Description = field.Description
			};
			return View(model);
		}

		/// <summary>
		/// Create a field
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditField(CreateTableFieldViewModel field)
		{
			var tableName = Context.Table.FirstOrDefault(x => x.Id == field.TableId)?.Name;
			if (tableName == null) return View();
			ITablesService sqlService = GetSqlService();
			switch (field.Parameter)
			{
				case FieldType.EntityReference:
					field.DataType = "uniqueidentifier";
					break;
				case FieldType.Boolean:
					FieldConfigViewModel defaultBool = null;
					foreach (var c in field.Configurations)
					{
						if (c.Name != FieldConfig.DefaultValue) continue;
						defaultBool = c;
						break;
					}

					if (defaultBool?.Value != null && defaultBool.Value.Trim() == "on") defaultBool.Value = "1";
					if (defaultBool?.Value != null && defaultBool.Value.Trim() == "off") defaultBool.Value = "0";
					break;
				case FieldType.DateTime:
				case FieldType.Date:
				case FieldType.Time:
					FieldConfigViewModel defaultTime = null;
					foreach (var c in field.Configurations)
					{
						if (c.Name != FieldConfig.DefaultValue) continue;
						defaultTime = c;
						break;
					}

					if (defaultTime?.Value != null && defaultTime.Value.Trim() == "on")
						defaultTime.Value = "CURRENT_TIMESTAMP";
					if (defaultTime?.Value != null && defaultTime.Value.Trim() == "off") defaultTime.Value = null;
					break;
			}

			//	var insertField = sqlService.AddFieldSql(field, tableName, _connectionString, false);
			// Save field model in the dataBase
			if (true)
			{
				var configValues = new List<TableFieldConfigValues>();
				var model = new TableModelFields
				{
					Id = field.Id,
					DataType = field.DataType,
					TableId = field.TableId,
					Description = field.Description,
					Name = field.Name,
					DisplayName = field.DisplayName,
					AllowNull = field.AllowNull,
					Synchronized = true,
					TableFieldTypeId = field.TableFieldTypeId,
				};
				foreach (var item in field.Configurations)
				{
					configValues.Add(new TableFieldConfigValues
					{
						TableFieldConfigId = item.ConfigId,
						TableModelFieldId = model.Id,
						Value = item.Value,
					});
				}

				model.TableFieldConfigValues = configValues;
				Repository.Refresh<TableModelFields, TableModelFields>(model);
				foreach (var item in model.TableFieldConfigValues)
				{
					Context.TableFieldConfigValues.Update(item);
					Context.SaveChanges();
				}

				return RedirectToAction("Edit", "Table", new { id = field.TableId, tab = "two" });
			}
		}

		/// <summary>
		/// Delete field
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public JsonResult DeleteField(string id)
		{
			var field = Context.TableFields.First(x => x.Id == Guid.Parse(id));
			var table = Context.Table.First(x => x.Id == field.TableId);
			ITablesService sqlService = GetSqlService();
			var checkColumn = sqlService.CheckColumnValues(ConnectionString.Item2, table.Name, table.EntityType, field.Name);
			if (checkColumn.Result)
			{
				return Json(false);
			}
			else
			{
				var fieldType = Context.TableFieldTypes.First(x => x.Name == FieldType.EntityReference).Id;
				if (field.TableFieldTypeId == fieldType)
				{
					// ?? posibil eroare
					var configtype = Context.TableFieldConfigs.First(x => x.TableFieldTypeId == fieldType).Id;
					var configValue = Context.TableFieldConfigValues.First(x =>
						x.TableFieldConfigId == configtype && x.TableModelFieldId == field.Id).Value;
					var dropConstraint = sqlService.DropConstraint(ConnectionString.Item2, table.Name, table.EntityType, configValue, field.Name);
				}

				var dropColumn = sqlService.DropColumn(ConnectionString.Item2, table.Name, table.EntityType, field.Name);
				if (!dropColumn.Result) return Json(false);
				Context.TableFields.Remove(field);
				Context.SaveChanges();
				return Json(true);
			}
		}

		/// <summary>
		/// Delete table
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableDelete)]
		public JsonResult DeleteTable(Guid? id)
		{
			if (!id.HasValue)
			{
				return Json(new { success = false, message = "Id not found" });
			}

			var table = Context.Table.First(x => x.Id == id);
			ITablesService sqlService = GetSqlService();
			var checkColumn = sqlService.CheckTableValues(ConnectionString.Item2, table.Name, table.EntityType);
			if (checkColumn.Result)
			{
				return Json(new { success = false, message = "Table has value" });
			}
			else
			{
				var isUsedForms = Context.Forms.Any(x => x.TableId == id);
				var isUsedProfiles = ContextApp.Profiles.Any(x => x.EntityId == id);
				if (isUsedForms || isUsedProfiles)
				{
					return Json(new { success = false, message = "Table is used" });
				}

				try
				{
					sqlService.DropTable(ConnectionString.Item2, table.Name, table.EntityType);
					Context.Table.Remove(table);
					Context.SaveChanges();
					return Json(new { success = true, message = "Delete success" });
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
					return Json(new { success = false, message = "Same error on delete!" });
				}
			}
		}
	}
	public static class FieldExtension
	{
		public static CreateTableFieldViewModel CreateSqlField(this CreateTableFieldViewModel field, (DbProviderType, string) connection)
		{
			switch (field.Parameter)
			{
				case FieldType.EntityReference:
					field.DataType = "uniqueidentifier";
					break;
				case FieldType.Boolean:
					FieldConfigViewModel defaultBool = null;
					foreach (var c in field.Configurations)
					{
						if (c.Name != FieldConfig.DefaultValue) continue;
						defaultBool = c;
						break;
					}

					if (defaultBool?.Value != null && defaultBool.Value.Trim() == "on") defaultBool.Value = "1";
					if (defaultBool?.Value != null && defaultBool.Value.Trim() == "off") defaultBool.Value = "0";
					break;
				case FieldType.DateTime:
				case FieldType.Date:
				case FieldType.Time:
					FieldConfigViewModel defaultTime = null;
					foreach (var c in field.Configurations)
					{
						if (c.Name != FieldConfig.DefaultValue) continue;
						defaultTime = c;
						break;
					}

					if (defaultTime?.Value != null && defaultTime.Value.Trim() == "on")
						defaultTime.Value = "CURRENT_TIMESTAMP";
					if (defaultTime?.Value != null && defaultTime.Value.Trim() == "off") defaultTime.Value = null;
					break;
				case FieldType.File:
					field.DataType = "uniqueidentifier";
					var foreignTable = field.Configurations.FirstOrDefault(s => s.Name == "ForeingTable");
					if (foreignTable != null)
					{
						foreignTable.Value = "FileReferences";
					}

					break;
			}
			return field;
		}
	}
}