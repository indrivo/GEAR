using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using Mapster;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace GR.Entities.Controls.Builders
{
    public static class ViewModelBuilderFactory
    {
        /// <summary>
        /// Build entity models
        /// </summary>
        private static readonly ConcurrentDictionary<string, EntityViewModel> EntityModels = new ConcurrentDictionary<string, EntityViewModel>();

        /// <summary>
        /// Reset build entity
        /// </summary>
        /// <param name="entityName"></param>
        public static void ResetBuildEntity(string entityName) => EntityModels.TryRemove(entityName, out _);


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
                .FirstOrDefault(d => d.Name == entityName.Trim() && d.EntityType.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA)
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

        /// <summary>
        /// Populate fields of table
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<EntityViewModel> ResolveAsync(EntitiesDbContext dbContext, EntityViewModel model)
        {
            return Task.Run(() => Resolve(dbContext, model));
        }

        /// <summary>
        /// Populate fields of table
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static EntityViewModel Resolve(EntitiesDbContext dbContext, EntityViewModel model)
        {
            Arg.NotNull(dbContext, nameof(EntitiesDbContext));
            Arg.NotNull(model, nameof(EntityViewModel));
            if (EntityModels.ContainsKey(model.TableName))
            {
                var storageObject = EntityModels[model.TableName];
                storageObject.TableSchema = model.TableSchema;
                storageObject.Values = model.Values;
                return storageObject;
            }

            var baseModelFields = BaseModelBuilder.CreateBaseModel(model.TableName).Adapt<List<TableModelField>>();
            foreach (var item in baseModelFields) item.IsSystem = true;

            foreach (var item in baseModelFields)
                model.Fields.Add(new EntityFieldsViewModel
                {
                    ColumnName = item.Name,
                    Type = item.DataType,
                    IsSystem = item.IsSystem
                });

            model.Fields.AddRange(dbContext.ViewModelConfig(model.TableName));
            EntityModels.TryAdd(model.TableName, model);
            return model;
        }
    }
}