using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Constants;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.Table;
using ST.Entities.Data;

namespace ST.Entities
{
    public class EntityRepository : IEntityRepository
    {
        private readonly EntitiesDbContext _context;

        public EntityRepository(EntitiesDbContext context)
        {
            _context = context;
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
                var foreignTable = await _context.Table.AsNoTracking().FirstOrDefaultAsync(x => x.Name == field.Configurations.FirstOrDefault(y => y.Name == FieldConfig.ForeingTable).Value);
                if (foreignSchema == null) return rs;
                if (foreignTable != null) foreignSchema.Value = foreignTable.EntityType;
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
            var entitiesList = await _context.Table.ToListAsync();
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
            var syncronizer = IoC.Resolve<EntitySynchronizer>();
            Arg.NotNull(syncronizer, nameof(EntitySynchronizer));
            var entitiesList = new List<SeedEntity>
            {
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "SysEntities.json")),
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/CustomEntities.json")),
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "ProfileEntities.json"))
            };

            foreach (var item in entitiesList)
            {
                if (item.SynchronizeTableViewModels == null) continue;
                foreach (var ent in item.SynchronizeTableViewModels)
                {
                    if (!await IoC.Resolve<EntitiesDbContext>().Table.AnyAsync(s => s.Name == ent.Name && s.TenantId == tenantId))
                    {
                        await syncronizer.SynchronizeEntities(ent, tenantId, schemaName);
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
                .Where(x => !x.IsCommon && !x.IsPartOfDbContext && x.EntityType.Equals(Settings.DEFAULT_ENTITY_SCHEMA))
                .ToListAsync();
            var syncModels = new List<SynchronizeTableViewModel>();
            foreach (var item in entities)
            {
                item.EntityType = schemaName;
                var tableConfig = await GetTableConfiguration(item.Id, item);
                if (!tableConfig.IsSuccess) return;
                var entity = tableConfig.Result;
                entity.Schema = schemaName;
                if (!await IoC.Resolve<EntitiesDbContext>().Table.AnyAsync(s => s.Name == entity.Name && s.TenantId == tenantId))
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
                            x.Name.Equals(tableName.Value) && x.EntityType == Settings.DEFAULT_ENTITY_SCHEMA);
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
                    if (!newFieldResult.IsSuccess)
                    {
                        Debug.WriteLine(newFieldResult.Errors);
                    }
                });
            });
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
