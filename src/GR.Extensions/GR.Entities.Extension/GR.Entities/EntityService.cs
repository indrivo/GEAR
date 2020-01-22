using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.ConnectionStrings;
using GR.Core.Helpers.Responses;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Events;
using GR.Entities.Abstractions.Events.EventArgs;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;
using GR.Entities.Data;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GR.Entities
{
    public class EntityService : IEntityService
    {
        #region Injectable
        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<EntityService> _logger;

        /// <summary>
        /// Inject table context
        /// </summary>
        private readonly EntitiesDbContext _context;

        /// <summary>
        /// Inject memory cache
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject table service builder
        /// </summary>
        private readonly ITablesService _tablesService;

        /// <summary>
        /// Inject configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        #endregion

        public EntityService(EntitiesDbContext context, IMemoryCache memoryCache, IUserManager<GearUser> userManager, ITablesService tablesService, IConfiguration configuration, ILogger<EntityService> logger)
        {
            _context = context;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _tablesService = tablesService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Table list
        /// </summary>
        public IQueryable<TableModel> Tables => _context.Table.AsNoTracking();

        /// <summary>
        /// Update table field configurations
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="viewConfigs"></param>
        /// <param name="dbConfigs"></param>
        /// <returns></returns>
        public virtual ResultModel UpdateTableFieldConfigurations(Guid fieldId, ICollection<FieldConfigViewModel> viewConfigs, ICollection<TableFieldConfigValue> dbConfigs)
        {
            var response = new ResultModel();
            var options = new ParallelOptions() { MaxDegreeOfParallelism = 3 };
            Parallel.ForEach(viewConfigs, options, async item =>
            {
                switch (item.ConfigCode)
                {
                    case TableFieldConfigCode.Reference.DisplayFormat:
                        {
                            await UpdateTableFieldDisplayFormatAsync(fieldId, item, dbConfigs);
                        }
                        break;
                }
            });
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Get configurations on add new field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<FieldConfigViewModel>>> RetrieveConfigurationsOnAddNewTableFieldAsyncTask(CreateTableFieldViewModel field)
        {
            Arg.NotNull(field, nameof(RetrieveConfigurationsOnAddNewTableFieldAsyncTask));
            var rs = new ResultModel<IEnumerable<FieldConfigViewModel>>();
            var data = field.Configurations ?? new List<FieldConfigViewModel>();

            var dbConfigs = await _context.TableFieldConfigs.AsNoTracking().Where(x => x.TableFieldTypeId == field.TableFieldTypeId).ToListAsync();

            var fieldTypeConfig = dbConfigs.Select(item => new FieldConfigViewModel
            {
                Name = item.Name,
                Type = item.Type,
                ConfigId = item.Id,
                Description = item.Description,
                ConfigCode = item.Code
            }).ToList();

            if (field.Parameter != FieldType.EntityReference) return rs;
            {
                var foreignSchema = fieldTypeConfig.FirstOrDefault(x => x.ConfigCode == TableFieldConfigCode.Reference.ForeingSchemaTable);
                var foreignTable = await _context.Table
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Name == field.Configurations
                                                  .FirstOrDefault(y => y.Name == FieldConfig.ForeingTable)
                                                  .Value);
                if (foreignSchema == null) return rs;
                if (foreignTable != null)
                {
                    foreignSchema.Value = foreignTable.IsPartOfDbContext ? foreignTable.EntityType : GearSettings.DEFAULT_ENTITY_SCHEMA;
                }
                var exist = data.FirstOrDefault(x =>
                    x.Name == nameof(TableFieldConfigCode.Reference.ForeingSchemaTable));
                if (exist == null)
                    data.Add(foreignSchema);
                else
                {
                    var index = data.IndexOf(exist);
                    data = data.Replace(index, foreignSchema).ToList();
                }
            }
            rs.Result = data;
            rs.IsSuccess = true;
            return rs;
        }

        /// <summary>
        /// Get add new field view model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreateTableFieldViewModel>> GetAddFieldCreateViewModel(Guid id, string type)
        {
            var rs = new ResultModel<CreateTableFieldViewModel>();
            var entitiesList = await _context.Table
                .Where(x => x.IsPartOfDbContext || x.EntityType.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA))
                .ToListAsync();
            if (!entitiesList.Any(x => x.Id.Equals(id)))
            {
                rs.Errors.Add(new ErrorModel("error", "Entity not found!"));
                return rs;
            }
            var fieldType = await _context.TableFieldTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Name == type.Trim());
            var fieldTypeConfig = _context.TableFieldConfigs.AsNoTracking().Where(x => x.TableFieldTypeId == fieldType.Id).ToList();

            var configurations = fieldTypeConfig.Select(item => new FieldConfigViewModel
            {
                Name = item.Name,
                Type = item.Type,
                ConfigId = item.Id,
                Description = item.Description,
                ConfigCode = item.Code
            }).ToList();

            var model = new CreateTableFieldViewModel
            {
                TableId = id,
                Configurations = configurations,
                TableFieldTypeId = fieldType.Id,
                DataType = fieldType.DataType,
                Parameter = type,
                EntitiesList = entitiesList.Select(x => x.Name).OrderBy(x => x).ToList()
            };
            rs.IsSuccess = true;
            rs.Result = model;
            return rs;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create dynamic tables
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="schemaName"></param>
        public async Task CreateDynamicTablesFromInitialConfigurationsFile(Guid tenantId, string schemaName = null)
        {
            var synchronizer = IoC.Resolve<EntitySynchronizer>();
            var context = IoC.Resolve<EntitiesDbContext>();
            Arg.NotNull(synchronizer, nameof(EntitySynchronizer));
            Arg.NotNull(context, nameof(EntitiesDbContext));
            var entitiesList = new List<SeedEntity>
            {
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/SysEntities.json")),
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/CustomEntities.json")),
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/ProfileEntities.json"))
            };

            foreach (var item in entitiesList)
            {
                if (item.SynchronizeTableViewModels == null) continue;
                foreach (var ent in item.SynchronizeTableViewModels)
                {
                    if (!await context.Table.AnyAsync(s => s.Name == ent.Name && s.TenantId == tenantId))
                    {
                        await synchronizer.SynchronizeEntities(ent, tenantId, schemaName);
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Create dynamic tables by replicate from system schema 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public async Task CreateDynamicTablesByReplicateSchema(Guid tenantId, string schemaName = null)
        {
            var synchronizer = IoC.Resolve<EntitySynchronizer>();
            Arg.NotNull(synchronizer, nameof(EntitySynchronizer));
            var entities = await _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldType)
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .Where(x => !x.IsCommon && !x.IsPartOfDbContext && x.EntityType.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA))
                .AsNoTracking()
                .ToListAsync();
            var syncModels = new List<SynchronizeTableViewModel>();
            foreach (var item in entities)
            {
                var vEntity = item;
                vEntity.EntityType = schemaName;
                var tableConfig = await GetTableConfiguration(vEntity.Id, vEntity);
                if (!tableConfig.IsSuccess) continue;
                var entity = tableConfig.Result;
                entity.Schema = schemaName;
                if (!await _context.Table.AnyAsync(s => s.Name == entity.Name && s.TenantId == tenantId))
                {
                    syncModels.Add(entity);
                }
            }

            await synchronizer.SynchronizeEntitiesByBeforeCreateTables(syncModels, tenantId, schemaName);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get table configuration
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        public async Task<ResultModel<SynchronizeTableViewModel>> GetTableConfiguration(Guid tableId, TableModel tableModel = null)
        {
            var result = new ResultModel<SynchronizeTableViewModel>();
            var table = tableModel ?? await _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldType)
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldType)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(tableId));
            if (table == null)
            {
                result.Errors.Add(new ErrorModel("", "Table not found"));
                return result;
            }

            var fields = await GetTableFieldsForBuildMode(table);
            var model = new SynchronizeTableViewModel
            {
                Name = table.Name,
                Description = table.Description,
                IsStaticFromEntityFramework = table.IsPartOfDbContext,
                IsSystem = table.IsSystem,
                Schema = table.EntityType,
                Fields = fields.ToList()
            };
            result.IsSuccess = true;
            result.Result = model;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get table field configurations
        /// </summary>
        /// <param name="field"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<FieldConfigViewModel>> GetTableFieldConfigurations(TableModelField field, string schema)
        {
            Arg.NotNull(field, nameof(TableModelField));
            var fieldType = await _context.TableFieldTypes.FirstOrDefaultAsync(x => x.Id == field.TableFieldTypeId);
            var fieldTypeConfig = _context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id).ToList();
            var configFields = new List<FieldConfigViewModel>();
            //TODO: if config not defined, load it
            foreach (var y in field.TableFieldConfigValues)
            {
                var fTypeConfig = fieldTypeConfig.FirstOrDefault(x => x.Id == y.TableFieldConfigId);
                if (fTypeConfig == null) continue;

                var config = new FieldConfigViewModel
                {
                    Name = fTypeConfig.Name,
                    Type = fTypeConfig.Type,
                    ConfigId = y.TableFieldConfigId,
                    Description = fTypeConfig.Description,
                    ConfigCode = fTypeConfig.Code,
                    Value = y.Value
                };

                if (fTypeConfig.Code.Equals(TableFieldConfigCode.Reference.ForeingSchemaTable))
                {
                    var tableName = field.TableFieldConfigValues
                        .FirstOrDefault(x => x.TableFieldConfig.Code
                            .Equals(TableFieldConfigCode.Reference.ForeingTable));
                    if (tableName != null)
                    {
                        var table = _context.Table.FirstOrDefault(x =>
                            x.Name.Equals(tableName.Value) && x.EntityType == GearSettings.DEFAULT_ENTITY_SCHEMA);
                        if (table != null && !table.IsPartOfDbContext && !table.IsSystem && !table.IsCommon)
                        {
                            config.Value = schema;
                        }
                    }
                }
                configFields.Add(config);
            }

            return configFields;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get table fields for builder mode
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<CreateTableFieldViewModel>> GetTableFieldsForBuildMode(TableModel table)
        {
            Arg.NotNull(table, nameof(TableModel));
            var fieldTypes = await _context.TableFieldTypes
                .Include(x => x.TableFieldGroups)
                .ToListAsync();
            var result = new List<CreateTableFieldViewModel>();

            foreach (var field in table.TableFields)
            {
                var fieldTypeName = fieldTypes.FirstOrDefault(u => u.DataType.Equals(field.DataType))?.Name;
                var configurations = await GetTableFieldConfigurations(field, table.EntityType);
                var model = new CreateTableFieldViewModel
                {
                    Id = field.Id,
                    Name = field.Name,
                    Description = field.Description,
                    AllowNull = field.AllowNull,
                    Parameter = fieldTypeName,
                    DataType = field.DataType,
                    DisplayName = field.DisplayName,
                    Configurations = configurations.ToList()
                };
                result.Add(model);
            }

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Duplicate tables for schema in database 
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual async Task DuplicateEntitiesForSchema(string schema)
        {
            var tableBuilder = IoC.Resolve<ITablesService>();
            var connection = _context.Database.GetDbConnection().ConnectionString;
            if (!_context.EntityTypes.Any(x => x.MachineName.ToLowerInvariant().Equals(schema.ToLowerInvariant()))) return;
            var entities = await _context.Table
                .Include(x => x.TableFields)
                    .ThenInclude(x => x.TableFieldType)
                .Include(x => x.TableFields)
                    .ThenInclude(x => x.TableFieldConfigValues)
                        .ThenInclude(x => x.TableFieldConfigId)
                .Where(x => !x.IsCommon && !x.IsPartOfDbContext)
                .ToListAsync();

            Parallel.ForEach(entities, async table =>
            {
                table.EntityType = schema;
                var dbTableResponse = tableBuilder.CreateSqlTable(table, connection);
                if (!dbTableResponse.IsSuccess) return;
                var fields = await GetTableFieldsForBuildMode(table);
                Parallel.ForEach(fields, y =>
                {
                    var newFieldResult = tableBuilder.AddFieldSql(y, table.Name, connection, true, schema);
                    if (!newFieldResult.IsSuccess) Debug.WriteLine(newFieldResult.Errors);
                });
            });
        }

        /// <summary>
        /// Generate tables
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> GenerateTablesForTenantAsync(Tenant model)
        {
            Arg.NotNull(model, nameof(GenerateTablesForTenantAsync));
            var response = new ResultModel();
            if (_context.EntityTypes.Any(x => x.MachineName == model.MachineName))
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Schema is used, try to use another"));
                return response;
            }

            _context.EntityTypes.Add(new EntityType
            {
                MachineName = model.MachineName,
                Author = nameof(System),
                Created = DateTime.Now,
                Changed = DateTime.Now,
                Name = model.MachineName,
                Description = $"Generated schema on created {model.Name} tenant"
            });
            var dbResult = await _context.SaveAsync();
            if (!dbResult.IsSuccess) return dbResult;


            await CreateDynamicTablesByReplicateSchema(model.Id, model.MachineName);
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Generate entity cache key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenerateEntityCacheKey(string name) => $"gear_dynamic_entity_{name}_cache_key";

        /// <summary>
        /// Find table by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<TableModel>> FindTableByNameAsync(string name, Func<TableModel, bool> filter = null)
        {
            var key = GenerateEntityCacheKey(name);

            if (filter == null)
                filter = x => x.Name.Equals(name) && x.TenantId == _userManager.CurrentUserTenantId
                     || x.Name.Equals(name) && x.IsCommon
                     || x.IsPartOfDbContext && x.Name.Equals(name);

            var tables = _memoryCache.Get<IEnumerable<TableModel>>(key)?.ToList() ?? new List<TableModel>();

            var table = tables.FirstOrDefault(filter.GetValueOrDefault());

            if (table != null) return new SuccessResultModel<TableModel>(table);

            var dbTable = await _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .ThenInclude(x => x.TableFieldType)
                .FirstOrDefaultAsync(filter.GetValueOrDefault().ToExpression());

            if (dbTable == null) return new NotFoundResultModel<TableModel>();

            tables.Add(dbTable);
            _memoryCache.Set(key, tables);
            return new SuccessResultModel<TableModel>(dbTable);
        }

        /// <summary>
        /// Delete table by id
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteTableAsync(Guid? tableId)
        {
            var result = new ResultModel();
            if (!tableId.HasValue) return new InvalidParametersResultModel<object>().ToBase();

            var table = await _context.Table.FirstOrDefaultAsync(x => x.Id == tableId);
            if (table == null) return new NotFoundResultModel();

            var (_, connection) = DbUtil.GetConnectionString(_configuration);

            var checkColumn = _tablesService.CheckTableValues(connection, table.Name, table.EntityType);
            if (checkColumn.Result)
            {
                result.Errors.Add(new ErrorModel
                {
                    Message = "The table contains data and cannot be deleted"
                });
                return result;
            }

            var dropResult = _tablesService.DropTable(connection, table.Name, table.EntityType);
            if (!dropResult.IsSuccess) return dropResult.ToBase();
            _context.Table.Remove(table);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess) return dbResult;
            _logger.LogInformation($"Table {table.Name} was deleted");
            EntityEvents.Entities.EntityDeleted(new EntityDeleteEventArgs
            {
                EntityId = tableId.Value,
                EntityName = table.Name
            });
            return dbResult;
        }


        #region Private Methods
        /// <summary>
        /// Update display format configuration for table field
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="target"></param>
        /// <param name="dbItems"></param>
        /// <returns></returns>
        private static async Task UpdateTableFieldDisplayFormatAsync(Guid fieldId, FieldConfigViewModel target, ICollection<TableFieldConfigValue> dbItems)
        {
            var context = IoC.Resolve<IEntityContext>();
            var existentConfig = dbItems.FirstOrDefault(x => x.TableFieldConfig?.Code == target.ConfigCode);
            if (existentConfig == null)
            {
                var confType =
                    await context.TableFieldConfigs.FirstOrDefaultAsync(x =>
                        x.Code == TableFieldConfigCode.Reference.DisplayFormat);
                context.TableFieldConfigValues.Add(new TableFieldConfigValue
                {
                    Value = target.Value,
                    TableFieldConfig = confType,
                    TableModelFieldId = fieldId
                });
            }
            else
            {
                existentConfig.Value = target.Value;
                context.TableFieldConfigValues.Remove(existentConfig);
                await context.SaveChangesAsync();
                context.TableFieldConfigValues.Add(existentConfig);
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion
    }
}
