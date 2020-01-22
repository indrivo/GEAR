using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GR.Core;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.ConnectionStrings;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.DynamicEntityStorage.Abstractions.Helpers;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Extensions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;
using GR.Entities.Data;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Data;
using GR.Identity.Data.Permissions;
using GR.Identity.Permissions.Abstractions.Attributes;
using GR.MultiTenant.Abstractions;
using GR.Notifications.Abstractions;

namespace GR.Entities.Razor.Controllers.Entity
{
    /// <inheritdoc />
    /// <summary />
    /// <summary>
    /// Forms manipulation
    /// </summary>
    public class TableController : BaseIdentityController<ApplicationDbContext, EntitiesDbContext, GearUser, GearRole, Tenant, INotify<GearRole>>
    {
        /// <summary>
        /// Inject entity repository
        /// </summary>
        private readonly IEntityService _entityService;

        /// <summary>
        /// Inject table service builder
        /// </summary>
        private readonly ITablesService _tablesService;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        /// <summary>
        /// Database connection string
        /// </summary>
        private string ConnectionString { get; set; }

        public TableController(UserManager<GearUser> userManager, RoleManager<GearRole> roleManager, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<GearRole> notify, IConfiguration configuration, IEntityService entityService, ITablesService tablesService, IOrganizationService<Tenant> organizationService) : base(userManager, roleManager, applicationDbContext, context, notify)
        {
            _entityService = entityService;
            _tablesService = tablesService;
            _organizationService = organizationService;
            var (_, connection) = DbUtil.GetConnectionString(configuration);
            ConnectionString = connection;
            Context.Validate();
        }

        /// <summary>
        /// Create table
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableCreate)]
        public IActionResult Create()
        {
            var schemes = Context.EntityTypes.Where(x => !x.IsDeleted).ToList();
            var defaultSchema = schemes.FirstOrDefault(x => x.MachineName.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA));
            if (defaultSchema == null) return NotFound();
            var model = new CreateTableViewModel
            {
                EntityTypes = schemes,
                SelectedTypeId = defaultSchema.Id
            };
            return View(model);
        }

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
                await Context.EntityTypes.FirstOrDefaultAsync(x => x.Id == model.SelectedTypeId);
            if (entityType == null) return View(model);

            var newTable = new CreateTableViewModel
            {
                Name = model.Name,
                EntityType = GearSettings.DEFAULT_ENTITY_SCHEMA,  //entityType.Name,
                Description = model.Description,
                TenantId = CurrentUserTenantId,
                IsCommon = model.IsCommon
            };
            var table = newTable.Adapt<TableModel>();
            await Context.Table.AddAsync(table);
            var dbResult = await Context.SaveAsync();
            if (dbResult.IsSuccess)
            {
                var response = _tablesService.CreateSqlTable(table, ConnectionString);
                if (response.Result)
                {
                    if (!table.IsCommon)
                    {
                        var tenants = _organizationService.GetAllTenants().Where(x => x.MachineName != GearSettings.DEFAULT_ENTITY_SCHEMA).ToList();
                        foreach (var tenant in tenants)
                        {
                            var tenantConfTable = table;
                            tenantConfTable.EntityType = tenant.MachineName;
                            _tablesService.CreateSqlTable(tenantConfTable, ConnectionString);
                        }
                    }
                    return RedirectToAction("Edit", "Table", new { id = table.Id, tab = "one" });
                }
            }
            else
            {
                ModelState.AppendResultModelErrors(dbResult.Errors);
                return View(model);
            }

            return View(model);
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
        /// <param name="isStatic"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderList(DTParameters param, bool isStatic = false)
        {
            var filtered = Context.Filter<TableModel>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount, x => x.IsPartOfDbContext.Equals(isStatic) && isStatic || x.EntityType == GearSettings.DEFAULT_ENTITY_SCHEMA && x.IsPartOfDbContext.Equals(isStatic));

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
            model.TableFields = await Context.TableFields.AsNoTracking().Where(x => x.TableId == table.Id).ToListAsync();
            model.Groups = await Context.TableFieldGroups.AsNoTracking().Include(s => s.TableFieldTypes).ToListAsync();
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
            if (!ModelState.IsValid) return View(model);
            Context.Table.Update(model.Adapt<TableModel>());
            var dbResult = Context.Save();
            if (dbResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index), "Table");
            }

            ModelState.AddModelError(string.Empty, "Something went wrong on server");
            return View(model);
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
            var data = await _entityService.GetAddFieldCreateViewModel(id, type);
            if (!data.IsSuccess) return NotFound();
            return View(data.Result);
        }

        /// <summary>
        /// Add new field to entity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddField(CreateTableFieldViewModel field)
        {
            var entitiesList = _entityService.Tables;
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

            var configurationsRq = await _entityService.RetrieveConfigurationsOnAddNewTableFieldAsyncTask(field);
            if (configurationsRq.IsSuccess)
            {
                field.Configurations = configurationsRq.Result.ToList();
            }

            field = field.CreateSqlField();
            var insertField = _tablesService.AddFieldSql(field, tableName, ConnectionString, true, schema);
            // Save field model in the dataBase
            if (!insertField.Result)
            {
                ModelState.AddModelError(string.Empty, "Fail to apply changes to database!");
                return View(field);
            }

            if (!table.IsCommon)
            {
                var isDynamic = true;
                var isReference = false;
                var referenceIsCommon = true;
                var tenants = _organizationService.GetAllTenants().Where(x => x.MachineName != GearSettings.DEFAULT_ENTITY_SCHEMA).ToList();
                if (field.Parameter == FieldType.EntityReference)
                {
                    isReference = true;
                    var referenceTableName = field.Configurations
                        .FirstOrDefault(x => x.Name == nameof(TableFieldConfigCode.Reference.ForeingTable))?.Value;

                    if (!referenceTableName.IsNullOrEmpty())
                    {
                        var refTable = await Context.Table.FirstOrDefaultAsync(x =>
                            x.Name.Equals(referenceTableName) && x.EntityType.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA)
                            || x.Name.Equals(referenceTableName) && x.IsPartOfDbContext);

                        if (refTable.IsPartOfDbContext) isDynamic = false;
                        else if (!refTable.IsCommon) referenceIsCommon = false;
                    }
                }

                foreach (var tenant in tenants)
                {
                    if (isDynamic && isReference && !referenceIsCommon)
                    {
                        var schemaConf = field.Configurations?.FirstOrDefault(x =>
                            x.ConfigCode.Equals(TableFieldConfigCode.Reference.ForeingSchemaTable));
                        if (schemaConf != null)
                        {
                            var index = field.Configurations.IndexOf(schemaConf);
                            schemaConf.Value = tenant.MachineName;
                            field.Configurations = field.Configurations.Replace(index, schemaConf).ToList();
                        }
                    }
                    _tablesService.AddFieldSql(field, tableName, ConnectionString, true, tenant.MachineName);
                }
            }

            var configs = field.Configurations.Select(item => new TableFieldConfigValue
            {
                TableFieldConfigId = item.ConfigId,
                Value = item.Value,
            }).ToList();

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
                TableFieldConfigValues = configs
            };

            Context.TableFields.Add(model);
            var result = await Context.SaveAsync();
            if (result.IsSuccess)
            {
                RefreshRuntimeTypes();
                return RedirectToAction("Edit", "Table", new { id = field.TableId, tab = "two" });
            }

            ModelState.AppendResultModelErrors(result.Errors);

            return View(field);
        }

        /// <summary>
        /// Refresh runtime types on entity change structure
        /// </summary>
        [NonAction]
        private static void RefreshRuntimeTypes()
        {
            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async token =>
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
            if (type == Guid.Empty || fieldId == Guid.Empty) return NotFound();
            var field = await Context.TableFields
                .Include(x => x.TableFieldConfigValues)
                .FirstOrDefaultAsync(x => x.Id == fieldId);
            if (field == null) return NotFound();
            var fieldType = await Context.TableFieldTypes.FirstOrDefaultAsync(x => x.Id == type);
            var fieldTypeConfig = Context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id).ToList();
            var configFields = field.TableFieldConfigValues
                .Select(y =>
                {
                    var fTypeConfig = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId);
                    return new FieldConfigViewModel
                    {
                        Name = fTypeConfig.Name,
                        Type = fTypeConfig.Type,
                        ConfigId = y.TableFieldConfigId,
                        Description = fTypeConfig.Description,
                        ConfigCode = fTypeConfig.Code,
                        Value = y.Value
                    };
                }).ToList();

            configFields.AddRange(fieldTypeConfig.Select(item => new FieldConfigViewModel
            {
                Name = item.Name,
                Type = item.Type,
                ConfigId = item.Id,
                Description = item.Description,
                ConfigCode = item.Code
            }).Where(x => configFields.FirstOrDefault(y => y.ConfigCode == x.ConfigCode) == null).ToList());

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
            var table = await Context.Table
                .Include(x => x.TableFields)
                .FirstOrDefaultAsync(x => x.Id == field.TableId && x.TableFields.FirstOrDefault(y => y.Id == field.Id) != null);

            if (table == null) return NotFound();
            var model = table.TableFields.FirstOrDefault(x => x.Id == field.Id);
            if (model == null) return NotFound();
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
            var fieldType = await Context.TableFieldTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DataType == field.DataType);
            var fieldTypeConfig = Context.TableFieldConfigs.AsNoTracking()
                .Where(x => x.TableFieldTypeId == fieldType.Id).ToList();

            var newConfigs = field.Configurations;
            var dbFieldConfigs = Context.TableFieldConfigValues.AsNoTracking()
                .Include(x => x.TableFieldConfig)
                .Include(x => x.TableModelField)
                .Where(x => x.TableModelFieldId == field.Id).ToList();
            field.Configurations = dbFieldConfigs
                .Select(y =>
                {
                    var conf = fieldTypeConfig.FirstOrDefault(x => x.Id == y.TableFieldConfigId);
                    return new FieldConfigViewModel
                    {
                        Name = conf?.Name,
                        Type = conf?.Type,
                        ConfigId = y.TableFieldConfigId,
                        Description = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId).Description,
                        Value = y.Value
                    };
                }).ToList();

            var updateStructure = _tablesService.AddFieldSql(field, table.Name, ConnectionString, false, table.EntityType);
            // Save field model structure in the dataBase
            if (!updateStructure.IsSuccess) return View(field);

            model.Description = field.Description;
            model.Name = field.Name;
            model.DisplayName = field.DisplayName;
            model.AllowNull = field.AllowNull;

            try
            {
                Context.TableFields.Update(model);
                Context.SaveChanges();
                _entityService.UpdateTableFieldConfigurations(model.Id, newConfigs, dbFieldConfigs);
                return RedirectToAction("Edit", "Table", new { id = field.TableId, tab = "two" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ModelState.AddModelError("Fail", ex.Message);
            }

            return View(field);
        }



        /// <summary>
        /// Delete field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> DeleteField([Required]string id)
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

            var checkColumn = _tablesService.CheckColumnValues(ConnectionString, table.Name, table.EntityType, field.Name);
            if (checkColumn.Result)
            {
                return Json(false);
            }

            var fieldType = Context.TableFieldTypes.FirstOrDefault(x => x.Name == FieldType.EntityReference);
            if (fieldType == null)
            {
                return Json(false);
            }

            var tenants = _organizationService.GetAllTenants()
                .Where(x => x.MachineName != GearSettings.DEFAULT_ENTITY_SCHEMA).ToList();

            if (field.TableFieldTypeId == fieldType.Id)
            {
                var configType = Context.TableFieldConfigs.FirstOrDefault(x => x.TableFieldTypeId == fieldType.Id);
                if (configType == null)
                {
                    var configValue = Context.TableFieldConfigValues.First(x =>
                        x.TableFieldConfigId == configType.Id && x.TableModelFieldId == field.Id).Value;
                    if (configValue != null)
                    {
                        _tablesService.DropConstraint(ConnectionString, table.Name, table.EntityType, configValue, field.Name);
                        if (!table.IsCommon)
                        {
                            foreach (var tenant in tenants)
                            {
                                _tablesService.DropConstraint(ConnectionString, table.Name, tenant.MachineName, configValue, field.Name);
                            }
                        }
                    }
                }
            }

            var dropColumn = _tablesService.DropColumn(ConnectionString, table.Name, table.EntityType, field.Name);
            if (!dropColumn.Result) return Json(false);
            foreach (var tenant in tenants)
            {
                _tablesService.DropColumn(ConnectionString, table.Name, tenant.MachineName, field.Name);
            }
            Context.TableFields.Remove(field);
            var updateResult = await Context.SaveAsync();
            if (!updateResult.IsSuccess) return Json(false);
            //Call to refresh runtime dynamic types
            RefreshRuntimeTypes();
            return Json(true);
        }



        #region Api

        /// <summary>
        /// Delete table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmTableDelete)]
        public async Task<JsonResult> DeleteTable([Required]Guid? id) => Json(await _entityService.DeleteTableAsync(id));

        #endregion
    }
}