using System.Collections.Generic;
using System.Linq;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;
using ST.Entities.Data;

namespace ST.Entities.Controls.Builders
{
    public static class ViewModelConfigBuilder
    {
        /// <summary>
        ///     Create Entity viewmodel Configuration structure by entity name
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static List<EntityFieldsViewModel> ViewModelConfig(this EntitiesDbContext dbContext, string entityName)
        {
            var model = new List<EntityFieldsViewModel>();
            var entity = dbContext.Table.FirstOrDefault(d => d.Name == entityName.Trim());
            var fields = dbContext.TableFields.Where(d => d.TableId == entity.Id).ToList();
            foreach (var item in fields)
            {
                var field = new EntityFieldsViewModel
                {
                    IsSystem = false,
                    ColumnName = item.Name,
                    Type = item.DataType
                };
                var fieldTypeConfig =
                    dbContext.TableFieldConfigs.Where(x => x.TableFieldTypeId == item.TableFieldTypeId);
                var configFields = dbContext.TableFieldConfigValues.Where(x => x.TableModelFieldId == item.Id);
                var configurations = new List<EntityConfigViewModel>();
                foreach (var configField in configFields)
                {
                    var fieldType = fieldTypeConfig.FirstOrDefault(x => x.Id == configField.TableFieldConfigId);
                    if (fieldType != null)
                    {
                        configurations.Add(new EntityConfigViewModel
                        {
                            Name = fieldType.Name,
                            Type = fieldType.Type,
                            Description = fieldType.Description,
                            Value = configField.Value
                        });
                    }
                }

                field.Configurations = configurations;
                model.Add(field);
            }

            return model;
        }
    }
}