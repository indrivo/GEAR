using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Data;
using ST.Entities.ViewModels.DynamicEntities;

namespace ST.Entities.Controls.Builders
{
    public static class ViewModelBuilder
    {
        public static EntityViewModel Create(EntitiesDbContext dbContext, TableModel table, bool hasConfig)
        {
            var model = new EntityViewModel
            {
                TableName = table.Name,
                TableSchema = table.EntityType,
                Fields = new List<EntityFieldsViewModel>()
            };


            var baseModelFields = BaseModelBuilder.CreateBaseModel(table.EntityType).Adapt<List<TableModelFields>>();
            foreach (var item in baseModelFields) item.IsSystem = true;

            List<TableModelFields> resultList;
            if (hasConfig)
            {
                model.Fields = dbContext.ViewModelConfig(model.TableName);
                resultList = baseModelFields;
            }
            else
            {
                resultList = baseModelFields
                    .Concat(table.TableFields ?? new List<TableModelFields>())
                    .ToList();
            }

            foreach (var item in resultList)
                model.Fields.Add(new EntityFieldsViewModel
                {
                    ColumnName = item.Name,
                    Type = item.DataType,
                    IsSystem = item.IsSystem
                });

            return model;
        }

        public static EntityViewModel Create(EntitiesDbContext dbContext, EntityViewModel model)
        {
            var baseModelFields = BaseModelBuilder.CreateBaseModel(model.TableName).Adapt<List<TableModelFields>>();
            foreach (var item in baseModelFields) item.IsSystem = true;


            foreach (var item in baseModelFields)
                model.Fields.Add(new EntityFieldsViewModel
                {
                    ColumnName = item.Name,
                    Type = item.DataType,
                    IsSystem = item.IsSystem
                });

            if (model.HasConfig) model.Fields = dbContext.ViewModelConfig(model.TableName);

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
            var baseModelFields = BaseModelBuilder.CreateBaseModel(model.TableName).Adapt<List<TableModelFields>>();
            foreach (var item in baseModelFields) item.IsSystem = true;


            foreach (var item in baseModelFields)
                model.Fields.Add(new EntityFieldsViewModel
                {
                    ColumnName = item.Name,
                    Type = item.DataType,
                    IsSystem = item.IsSystem
                });

            model.Fields.AddRange(dbContext.ViewModelConfig(model.TableName));

            return model;
        }
    }
}