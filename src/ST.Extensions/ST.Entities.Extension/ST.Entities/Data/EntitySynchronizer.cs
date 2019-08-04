using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.Table;

namespace ST.Entities.Data
{
    public class EntitySynchronizer
    {
        private readonly string _connectionString;
        private readonly EntitiesDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
		public EntitySynchronizer(EntitiesDbContext context)
        {
            _context = context;
            _connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        /// <summary>
		/// Sync entities
		/// </summary>
		/// <param name="tableModel"></param>
		/// <param name="tenantId"></param>
		/// <param name="schema"></param>
		public async Task SynchronizeEntities(SynchronizeTableViewModel tableModel, Guid? tenantId, string schema = null)
        {
            var table = new TableModel
            {
                Name = tableModel.Name,
                EntityType = schema ?? tableModel.Schema,
                Description = tableModel.Description,
                IsSystem = tableModel.IsSystem,
                IsPartOfDbContext = tableModel.IsStaticFromEntityFramework,
                TenantId = tenantId
            };
            _context.Table.Add(table);
            var dbResult = await _context.SaveAsync();
            if (dbResult.IsSuccess)
            {
                await CompleteSyncEntityAsync(tableModel, table);
            }
            else
            {
                Debug.WriteLine(dbResult.Errors);
            }
        }

        /// <summary>
        /// Complete sync entity
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="table"></param>
        private async Task CompleteSyncEntityAsync(SynchronizeTableViewModel tableModel, TableModel table)
        {
            var resultModel = await _context.Table
                .AsNoTracking()
                .Include(x => x.TableFields)
                .FirstOrDefaultAsync(x => x.Id == table.Id);

            if (resultModel == null) return;
            {
                if (tableModel.IsStaticFromEntityFramework)
                {
                    var fieldTypeList = await _context.TableFieldTypes.ToListAsync();
                    var fieldConfigList = await _context.TableFieldConfigs.ToListAsync();
                    foreach (var item in tableModel.Fields)
                    {
                        if (item.Configurations != null)
                            foreach (var configViewModel in item.Configurations)
                            {
                                configViewModel.Name = fieldConfigList.FirstOrDefault(x => x.Code == configViewModel.ConfigCode)?.Name;
                            }
                        // Save field model in the dataBase
                        var configValues = new List<TableFieldConfigValue>();
                        var fieldType = fieldTypeList.FirstOrDefault(x => x.Code == item.TableFieldCode);
                        if (fieldType == null) continue;
                        var model = new TableModelField
                        {
                            DataType = item.DataType,
                            DisplayName = item.DisplayName,
                            TableId = resultModel.Id,
                            Description = item.Description,
                            Name = item.Name,
                            AllowNull = item.AllowNull,
                            Synchronized = true,
                            TableFieldTypeId = fieldType.Id,
                        };
                        if (item.Configurations != null)
                            foreach (var configItem in item.Configurations)
                            {
                                var config = fieldConfigList.FirstOrDefault(x => x.Code == configItem.ConfigCode);
                                if (config == null) continue;

                                configValues.Add(new TableFieldConfigValue
                                {
                                    TableFieldConfigId = config.Id,
                                    TableModelFieldId = model.Id,
                                    Value = configItem.Value,
                                });
                            }
                        model.TableFieldConfigValues = configValues;
                        _context.TableFields.Add(model);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    var sqlService = IoC.Resolve<ITablesService>();

                    if (sqlService == null) return;
                    var response = sqlService.CreateSqlTable(table: resultModel, connectionString: _connectionString);
                    if (!response.Result) return;
                    var fieldTypeList = _context.TableFieldTypes.ToList();
                    var fieldConfigList = _context.TableFieldConfigs.ToList();

                    foreach (var item in tableModel.Fields)
                    {
                        foreach (var configViewModel in item.Configurations)
                        {
                            configViewModel.Name = fieldConfigList.FirstOrDefault(x => x.Code == configViewModel.ConfigCode)?.Name;

                            if (configViewModel.ConfigCode == "9999")
                            {
                                if (configViewModel.Value == "systemcore")
                                {
                                    configViewModel.Value = table.EntityType;
                                }
                            }
                        }

                        var insertField = sqlService.AddFieldSql(item, tableModel.Name, _connectionString, true, table.EntityType);
                        // Save field model in the dataBase
                        if (!insertField.Result) continue;
                        {
                            var configValues = new List<TableFieldConfigValue>();
                            var tableFieldType = fieldTypeList.FirstOrDefault(x => x.Code == item.TableFieldCode);
                            if (tableFieldType == null) continue;
                            var model = new TableModelField
                            {
                                DataType = item.DataType,
                                TableId = resultModel.Id,
                                Description = item.Description,
                                Name = item.Name,
                                DisplayName = item.DisplayName,
                                AllowNull = item.AllowNull,
                                Synchronized = true,
                                TableFieldTypeId = tableFieldType.Id
                            };

                            foreach (var configItem in item.Configurations)
                            {
                                var config = fieldConfigList.FirstOrDefault(x => x.Code == configItem.ConfigCode);
                                if (config == null) continue;
                                configValues.Add(new TableFieldConfigValue
                                {
                                    TableFieldConfigId = config.Id,
                                    TableModelFieldId = model.Id,
                                    Value = configItem.Value,
                                });
                            }

                            model.TableFieldConfigValues = configValues;
                            _context.TableFields.Add(model);
                            var dbResult = await _context.SaveAsync();
                            if (!dbResult.IsSuccess)
                            {
                                Debug.WriteLine(dbResult.Errors);
                            }
                        }
                    }
                }
            }
        }
    }
}