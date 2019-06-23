using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Core.Helpers;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Constants;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.Table;

namespace ST.Entities
{
    public class EntityRepository : IEntityRepository
    {
        /// <summary>
        /// Update table field configurations
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="viewConfigs"></param>
        /// <param name="dbConfigs"></param>
        /// <returns></returns>
        public ResultModel UpdateTableFieldConfigurations(Guid fieldId, ICollection<FieldConfigViewModel> viewConfigs, ICollection<TableFieldConfigValue> dbConfigs)
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
    }
}
