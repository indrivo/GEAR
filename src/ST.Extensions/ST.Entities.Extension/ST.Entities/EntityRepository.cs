using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
