using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Entities.Data;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Entities.Abstractions;

namespace GR.Entities.Controls.Builders
{
    public static class ViewModelBuilderFactory
    {
        /// <summary>
        ///     Create Entity viewmodel Configuration structure by entity name
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static async Task<List<EntityFieldsViewModel>> InjectNonBaseMetaDataAsync(this EntitiesDbContext dbContext, string entityName)
        {
            var service = IoC.Resolve<IEntityService>();

            var model = new List<EntityFieldsViewModel>();
            if (entityName.IsNullOrEmpty()) return model;
            var entityRequest = await service.FindTableByNameAsync(entityName, d => d.Name == entityName.Trim()
                                                                                    && d.EntityType.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA)
                                                                                    || d.Name == entityName.Trim() && d.IsPartOfDbContext);
            if (!entityRequest.IsSuccess) return model;
            var entity = entityRequest.Result;
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

        /// <summary>
        /// Populate fields of table
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<EntityViewModel> ResolveAsync(EntitiesDbContext dbContext, EntityViewModel model)
        {
            Arg.NotNull(dbContext, nameof(EntitiesDbContext));
            Arg.NotNull(model, nameof(EntityViewModel));

            var baseModelFields = BaseModelBuilder.CreateBaseModel(model.TableName).Adapt<List<TableModelField>>();
            foreach (var item in baseModelFields) item.IsSystem = true;

            foreach (var item in baseModelFields)
                model.Fields.Add(new EntityFieldsViewModel
                {
                    ColumnName = item.Name,
                    Type = item.DataType,
                    IsSystem = item.IsSystem
                });

            model.Fields.AddRange(await dbContext.InjectNonBaseMetaDataAsync(model.TableName));
            return model;
        }
    }
}