using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Entities.Data;

namespace GR.Entities.Controls.Builders
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
            if (entityName.IsNullOrEmpty()) return model;
            var entity = dbContext.Table
                .Include(x => x.TableFields)
                .FirstOrDefault(d => d.Name == entityName.Trim() && d.EntityType.Equals(Settings.DEFAULT_ENTITY_SCHEMA)
                || d.Name == entityName.Trim() && d.IsPartOfDbContext);

            if (entity == null) return model;
            foreach (var item in entity.TableFields)
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