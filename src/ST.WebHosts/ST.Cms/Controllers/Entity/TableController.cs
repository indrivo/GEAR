using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ST.Cache.Abstractions;
using ST.Core;
using ST.Core.Abstractions;
using ST.Core.BaseControllers;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Services;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Constants;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.Table;
using ST.Entities.Data;
using ST.Entities.Utils;
using ST.Forms.Abstractions;
using ST.Identity.Abstractions;
using ST.Identity.Attributes;
using ST.Identity.Data;
using ST.Identity.Data.MultiTenants;
using ST.Identity.Data.Permissions;
using ST.Notifications.Abstractions;

namespace ST.Cms.Controllers.Entity
{
	/// <inheritdoc />
	/// <summary />
	/// <summary>
	/// Forms manipulation
	/// </summary>
	public class TableController : BaseController<ApplicationDbContext, EntitiesDbContext, ApplicationUser, ApplicationRole, Tenant, INotify<ApplicationRole>>
	{
		/// <summary>
		/// Inject logger
		/// </summary>
		private readonly ILogger<TableController> _logger;

		/// <summary>
		/// Queue for run background tasks
		/// </summary>
		private IBackgroundTaskQueue Queue { get; }

		/// <summary>
		/// Inject form context
		/// </summary>
		private readonly IFormContext _formContext;


		public TableController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify, ILogger<TableController> logger, IHostingEnvironment env, IConfiguration configuration, IBackgroundTaskQueue queue, IFormContext formContext) : base(userManager, roleManager, cacheService, applicationDbContext, context, notify)
		{
			_logger = logger;
			Queue = queue;
			_formContext = formContext;
			ConnectionString = DbUtil.GetConnectionString(configuration, env);
			formContext.ValidateNullAbstractionContext();
			Context.Validate();
		}

		private (DbProviderType, string) ConnectionString { get; set; }

		/// <summary>
		/// Create table
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableCreate)]
		public IActionResult Create()
		{
			var model = new CreateTableViewModel
			{
				EntityTypes = Context.EntityTypes.Where(x => !x.IsDeleted).ToList()
			};
			return View(model);
		}

		/// <summary>
		/// Get table dataService
		/// </summary>
		/// <returns></returns>
		[NonAction]
		private static ITablesService GetSqlService() => IoC.Resolve<ITablesService>();

		/// <summary>
		/// Create a table
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableCreate)]
		public async Task<IActionResult> Create(CreateTableViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			var entityType =
				await Context.EntityTypes.FirstOrDefaultAsync(x => x.Id == Guid.Parse(model.SelectedTypeId));
			if (entityType == null) return View(model);
			{
				var m = new CreateTableViewModel
				{
					Name = model.Name,
					EntityType = entityType.Name,
					Description = model.Description,
					TenantId = CurrentUserTenantId
				};
				try
				{
					var table = m.Adapt<TableModel>();
					await Context.Table.AddAsync(table);
					await Context.SaveChangesAsync();
					var sqlService = GetSqlService();
					var response = sqlService.CreateSqlTable(table, ConnectionString.Item2);
					if (response.Result)
						return RedirectToAction("Edit", "Table", new { id = table.Id, tab = "one" });
				}
				catch (Exception e)
				{
					ModelState.AddModelError("fail", e.Message);
					return View(model);
				}

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
				IsPartOfDbContext = o.IsPartOfDbContext,
				EntityType = o.EntityType,
				Changed = o.Changed,
				IsSystem = o.IsSystem,
				TableFields = o.TableFields
			});

			var finalResult = new DTResult<TableModel>
			{
				Draw = param.Draw,
				Data = orderList.ToList(),
				RecordsFiltered = totalCount,
				RecordsTotal = filtered.Count
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
			var table = await Context.Table.FirstOrDefaultAsync(x => x.Id == id);
			if (table == null) return NotFound();
			var model = table.Adapt<UpdateTableViewModel>();
			model.TableFields = await Context.TableFields.Where(x => x.TableId == table.Id).ToListAsync();
			if (model.ModifiedBy != null)
				model.ModifiedBy = ApplicationDbContext.Users
				.SingleOrDefaultAsync(m => m.Id == model.ModifiedBy).Result.NormalizedUserName.ToLower();
			model.Groups = await Context.TableFieldGroups.Include(s => s.TableFieldTypes).ToListAsync();
			model.TabName = tab;
			return View(model);
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

			try
			{
				Context.Table.Update(model.Adapt<TableModel>());
				Context.SaveChanges();
				return RedirectToAction(nameof(Index), "Table");
			}
			catch
			{
				ModelState.AddModelError(string.Empty, "Something went wrong on server");
				return View(model);
			}
		}

		/// <summary>
		/// Change status is deleted in true
		/// </summary>
		/// <param name="tableId"></param>
		/// <returns></returns>
		[HttpDelete]
		public JsonResult RemoveTableModel(Guid tableId)
		{
			var response = new ResultModel
			{
				Errors = new List<IErrorModel>()
			};
			var table = Context.Table.FirstOrDefault(x => x.Id == tableId);
			if (table == null)
			{
				response.Errors.Add(new ErrorModel("fail", "Entity not found!"));
				return Json(response);
			}
			try
			{
				Context.Table.Remove(table);
				Context.SaveChanges();
				response.IsSuccess = true;
			}
			catch
			{
				response.Errors.Add(new ErrorModel("fail", "Fail to save data"));
			}
			return Json(response);
		}

		/// <summary>
		/// Get view for add field
		/// </summary>
		/// <param name="id"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddField(Guid id, string type)
		{
			var entitiesList = await Context.Table.ToListAsync();
			var fieldType = await Context.TableFieldTypes.FirstOrDefaultAsync(x => x.Name == type.Trim());
			var fieldTypeConfig = Context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id);
			var configurations = new List<FieldConfigViewModel>();
			foreach (var item in fieldTypeConfig)
			{
				if (item.Code != "9999")
					configurations.Add(new FieldConfigViewModel
					{
						Name = item.Name,
						Type = item.Type,
						ConfigId = item.Id,
						Description = item.Description,
						ConfigCode = item.Code
					});
			}

			var model = new CreateTableFieldViewModel
			{
				TableId = id,
				Configurations = configurations,
				TableFieldTypeId = fieldType.Id,
				DataType = fieldType.DataType,
				Parameter = type,
				EntitiesList = entitiesList.Select(x => x.Name).ToList()
			};

			return View(model);
		}

		/// <summary>
		/// Add new field to entity
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> AddField(CreateTableFieldViewModel field)
		{
			var entitiesList = await Context.Table.ToListAsync();
			var table = entitiesList.FirstOrDefault(x => x.Id == field.TableId);
			var tableName = table?.Name;
			var schema = table?.EntityType;
			field.EntitiesList = entitiesList.Select(x => x.Name).ToList();
			if (table == null)
			{
				ModelState.AddModelError(string.Empty, "Table not found");
				return View(field);
			}

			var baseEntityProps = BaseModel.GetPropsName().Select(x => x.ToLower()).ToList();
			if (baseEntityProps.Contains(field.Name.Trim().ToLower()))
			{
				ModelState.AddModelError(string.Empty, "This field name can't be used, is system name!");
				return View(field);
			}

			var fieldTypeConfig = Context.TableFieldConfigs.Where(x => x.TableFieldTypeId == field.TableFieldTypeId).Select(item => new FieldConfigViewModel
			{
				Name = item.Name,
				Type = item.Type,
				ConfigId = item.Id,
				Description = item.Description,
				ConfigCode = item.Code
			}).ToList();

			if (field.Parameter == FieldType.EntityReference)
			{
				var foreignSchema = fieldTypeConfig.FirstOrDefault(x => x.ConfigCode == "9999");
				var foreignTable = Context.Table.FirstOrDefault(x => x.Name == field.Configurations.FirstOrDefault(y => y.Name == FieldConfig.ForeingTable).Value);
				if (foreignSchema != null)
				{
					if (foreignTable != null) foreignSchema.Value = foreignTable.EntityType;
					field.Configurations.Add(foreignSchema);
				}
			}

			var sqlService = GetSqlService();
			field = field.CreateSqlField();
			var insertField = sqlService.AddFieldSql(field, tableName, ConnectionString.Item2, true, schema);
			// Save field model in the dataBase
			if (!insertField.Result)
			{
				ModelState.AddModelError(string.Empty, "Fail to apply changes to database!");
				return View(field);
			}
			var configValues = new List<TableFieldConfigValue>();
			var model = new TableModelField
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
				configValues.Add(new TableFieldConfigValue
				{
					TableFieldConfigId = item.ConfigId,
					TableModelFieldId = model.Id,
					Value = item.Value,
				});
			}

			model.TableFieldConfigValues = configValues;

			try
			{
				Context.TableFields.Add(model);
				Context.SaveChanges();
				RefreshRuntimeTypes();
				return RedirectToAction("Edit", "Table", new { id = field.TableId, tab = "two" });
			}
			catch
			{
				ModelState.AddModelError(string.Empty, "Fail to apply changes to database!");
			}


			return View(field);
		}

		/// <summary>
		/// Refresh runtime types on entity change structure
		/// </summary>
		[NonAction]
		private void RefreshRuntimeTypes()
		{
			Queue.PushBackgroundWorkItemInQueue(async token =>
			{
				//TODO: Need to update only edited dynamic runtime type
				TypeManager.Clear();
				var dynService = IoC.Resolve<IDynamicService>();
				await dynService.RegisterInMemoryDynamicTypesAsync();
			});
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
			var configFields = Context.TableFieldConfigValues
				.Where(x => x.TableModelFieldId == fieldId)
				.Select(y => new FieldConfigViewModel
				{
					Name = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Name,
					Type = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Type,
					ConfigId = y.TableFieldConfigId,
					Description = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Description,
					Value = y.Value
				}).ToList();

			var model = new CreateTableFieldViewModel
			{
				Id = fieldId,
				TableId = field.TableId,
				Configurations = configFields,
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
		public async Task<IActionResult> EditField([Required]CreateTableFieldViewModel field)
		{
			var table = Context.Table.FirstOrDefault(x => x.Id == field.TableId);
			if (table == null) return NotFound();
			if (field.Id == Guid.Empty) return NotFound();
			switch (field.Parameter)
			{
				case FieldType.EntityReference:
					field.DataType = TableFieldDataType.Guid;
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
			var fieldType = await Context.TableFieldTypes.FirstOrDefaultAsync(x => x.DataType == field.DataType);
			var fieldTypeConfig = Context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id);
			field.Configurations = Context.TableFieldConfigValues
				.Where(x => x.TableModelFieldId == field.Id)
				.Select(y => new FieldConfigViewModel
				{
					Name = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Name,
					Type = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Type,
					ConfigId = y.TableFieldConfigId,
					Description = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Description,
					Value = y.Value
				}).ToList();
			var sqlService = GetSqlService();
			var updateStructure = sqlService.AddFieldSql(field, table.Name, ConnectionString.Item2, false, table.EntityType);
			// Save field model structure in the dataBase
			if (updateStructure.IsSuccess)
			{
				var configValues = new List<TableFieldConfigValue>();
				var model = new TableModelField
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
					configValues.Add(new TableFieldConfigValue
					{
						TableFieldConfigId = item.ConfigId,
						TableModelFieldId = model.Id,
						Value = item.Value,
					});
				}

				model.TableFieldConfigValues = configValues;
				try
				{
					Context.TableFields.Update(model);
					Context.SaveChanges();
					return RedirectToAction("Edit", "Table", new { id = field.TableId, tab = "two" });
				}
				catch
				{
					ModelState.AddModelError("Fail", "Fail on update data to database!");
				}

				//foreach (var item in model.TableFieldConfigValues)
				//{
				//	Context.TableFieldConfigValues.Update(item);
				//	Context.SaveChanges();
				//}
			}

			return View(field);
		}

		/// <summary>
		/// Delete field
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public JsonResult DeleteField([Required]string id)
		{
			var field = Context.TableFields.FirstOrDefault(x => x.Id == Guid.Parse(id));
			if (field == null)
			{
				return Json(false);
			}
			var table = Context.Table.FirstOrDefault(x => x.Id == field.TableId);
			if (table == null)
			{
				return Json(false);
			}
			ITablesService sqlService = GetSqlService();
			var checkColumn = sqlService.CheckColumnValues(ConnectionString.Item2, table.Name, table.EntityType, field.Name);
			if (checkColumn.Result)
			{
				return Json(false);
			}

			var fieldType = Context.TableFieldTypes.FirstOrDefault(x => x.Name == FieldType.EntityReference);
			if (fieldType == null)
			{
				return Json(false);
			}
			if (field.TableFieldTypeId == fieldType.Id)
			{
				// ?? posibil eroare
				var configtype = Context.TableFieldConfigs.First(x => x.TableFieldTypeId == fieldType.Id).Id;
				var configValue = Context.TableFieldConfigValues.First(x =>
					x.TableFieldConfigId == configtype && x.TableModelFieldId == field.Id).Value;
				sqlService.DropConstraint(ConnectionString.Item2, table.Name, table.EntityType, configValue, field.Name);
			}

			var dropColumn = sqlService.DropColumn(ConnectionString.Item2, table.Name, table.EntityType, field.Name);
			if (!dropColumn.Result) return Json(false);
			Context.TableFields.Remove(field);
			Context.SaveChanges();
			//Call to refresh runtime dynamic types
			RefreshRuntimeTypes();
			return Json(true);
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

			var isUsedForms = _formContext.Forms.Any(x => x.TableId == id);
			var isUsedProfiles = ApplicationDbContext.Profiles.Any(x => x.EntityId == id);
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
	public static class FieldExtension
	{
		public static CreateTableFieldViewModel CreateSqlField(this CreateTableFieldViewModel field)
		{
			switch (field.Parameter)
			{
				case FieldType.EntityReference:
					field.DataType = TableFieldDataType.Guid;
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
					field.DataType = TableFieldDataType.Guid;
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