using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Services;
using ST.Entities.Services.Abstraction;
using ST.Entities.ViewModels.Table;

namespace ST.Entities.Data
{
    public class EntitySynchronizer
    {
        private readonly string _connectionString;
        private readonly EntitiesDbContext _context;
        private readonly EntitiesDbContext _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="context"></param>
		public EntitySynchronizer(EntitiesDbContext repository,
            EntitiesDbContext context)
        {
            _repository = repository;
            _context = context;
            _connectionString = _context.Database.GetDbConnection().ConnectionString;
        }


        /// <summary>
		/// Sync entities
		/// </summary>
		/// <param name="tableModel"></param>
		/// <param name="tenantId"></param>
		/// <param name="schema"></param>
		public void SynchronizeEntities(SynchronizeTableViewModel tableModel, Guid? tenantId, string schema = null)
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
            try
            {
                _context.Table.Add(table);
                _context.SaveChanges();
                CompleteSyncEntity(tableModel, table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Complete sync entity
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="table"></param>
        private void CompleteSyncEntity(SynchronizeTableViewModel tableModel, TableModel table)
        {
            var resultModel = _repository.Table
                .Include(x => x.TableFields)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == table.Id);

            if (resultModel == null) return;
            {
                if (tableModel.IsStaticFromEntityFramework)
                {
                    var fieldTypeList = _context.TableFieldTypes.ToList();
                    var fieldConfigList = _context.TableFieldConfigs.ToList();
                    foreach (var item in tableModel.Fields)
                    {
                        if (item.Configurations != null)
                            foreach (var configViewModel in item.Configurations)
                            {
                                configViewModel.Name = fieldConfigList.Single(x => x.Code == configViewModel.ConfigCode).Name;
                            }
                        // Save field model in the dataBase
                        var configValues = new List<TableFieldConfigValues>();
                        var fieldTypeId = fieldTypeList.Single(x => x.Code == item.TableFieldCode).Id;
                        var model = new TableModelFields
                        {
                            DataType = item.DataType,
                            DisplayName = item.DisplayName,
                            TableId = resultModel.Id,
                            Description = item.Description,
                            Name = item.Name,
                            AllowNull = item.AllowNull,
                            Synchronized = true,
                            TableFieldTypeId = fieldTypeId,
                        };
                        if (item.Configurations != null)
                            foreach (var configItem in item.Configurations)
                            {
                                var configId = fieldConfigList.Single(x => x.Code == configItem.ConfigCode).Id;
                                configValues.Add(new TableFieldConfigValues
                                {
                                    TableFieldConfigId = configId,
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
                    ITablesService sqlService = _context.Database.IsNpgsql()
                        ? new NpgTablesService() : _context.Database.IsSqlServer()
                            ? new TablesService() : null;

                    if (sqlService == null) return;
                    var response = sqlService.CreateSqlTable(table: resultModel, connectionString: _connectionString);
                    if (!response.Result) return;
                    // Add
                    var fieldTypeList = _context.TableFieldTypes.ToList();
                    var fieldConfigList = _context.TableFieldConfigs.ToList();

                    foreach (var item in tableModel.Fields)
                    {
                        foreach (var configViewModel in item.Configurations)
                        {
                            configViewModel.Name = fieldConfigList.Single(x => x.Code == configViewModel.ConfigCode).Name;

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
                            var configValues = new List<TableFieldConfigValues>();
                            var fieldTypeId = fieldTypeList.Single(x => x.Code == item.TableFieldCode).Id;
                            var model = new TableModelFields
                            {
                                DataType = item.DataType,
                                TableId = resultModel.Id,
                                Description = item.Description,
                                Name = item.Name,
                                DisplayName = item.DisplayName,
                                AllowNull = item.AllowNull,
                                Synchronized = true,
                                TableFieldTypeId = fieldTypeId,
                            };
                            foreach (var configItem in item.Configurations)
                            {
                                var configId = fieldConfigList.Single(x => x.Code == configItem.ConfigCode).Id;
                                configValues.Add(new TableFieldConfigValues
                                {
                                    TableFieldConfigId = configId,
                                    TableModelFieldId = model.Id,
                                    Value = configItem.Value,
                                });
                            }
                            model.TableFieldConfigValues = configValues;
                            _context.TableFields.Add(model);
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}