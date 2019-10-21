using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using GR.Core;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.Abstractions
{
    public abstract class TablesService : ITablesService
    {
        public abstract ResultModel<bool> CreateSqlTable(TableModel table, string connectionString);

        public abstract ResultModel<bool> AddFieldSql(CreateTableFieldViewModel table, string tableName,
            string connectionString, bool isNew,
            string tableSchema);

        public abstract ResultModel<bool> CheckColumnValues(string connectionString, string tableName, string tableSchema,
            string columnName);

        public abstract ResultModel<bool> DropColumn(string connectionString, string tableName, string tableSchema,
            string columnName);

        public abstract ResultModel<bool> DropConstraint(string connectionString, string tableName, string tableSchema,
            string constraint,
            string columnName);

        public abstract ResultModel<bool> CheckTableValues(string connectionString, string tableName,
            string tableSchema);

        public abstract ResultModel<bool> DropTable(string connectionString, string tableName, string tableSchema);
        public abstract Task CreateSchemaAsync(string schemaName, string connectionString);

        public abstract IEnumerable<string> GetSchemas(string connectionString);


        /// <summary>
        /// Get static entities from contexts
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns></returns>
        public virtual IEnumerable<SynchronizeTableViewModel> GetEntitiesFromDbContexts(params Type[] contexts)
        {
            var result = new List<SynchronizeTableViewModel>();
            if (!contexts.Any()) return result;
            foreach (var context in contexts)
            {
                var entities = GetEntitiesFromContext(context).ToList();
                if (!entities.Any()) continue;
                result.AddRange(entities);
            }
            return result;
        }

        /// <summary>
        /// Get entities
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IEnumerable<SynchronizeTableViewModel> GetEntitiesFromContext(Type context)
        {
            var result = new List<SynchronizeTableViewModel>();
            var props = context.GetProperties().Where(x => x.PropertyType.IsGenericType).ToList();

            foreach (var prop in props)
            {
                var get = GetEntityData(prop, context);
                if (get == null) continue;
                result.Add(get);
            }
            return result;
        }

        /// <summary>
        /// Get entity data
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual SynchronizeTableViewModel GetEntityData(PropertyInfo prop, Type context)
        {
            var result = new SynchronizeTableViewModel();
            var fields = new List<CreateTableFieldViewModel>();
            var baseProps = BaseModel.GetPropsName().ToList();
            var entity = prop.PropertyType.GenericTypeArguments[0];

            if (entity.Name == "ApplicationUser")
            {
                entity = typeof(IdentityUser);
            }

            //if (entity.BaseType != typeof(BaseModel) || entity.BaseType != typeof(ExtendedModel)) continue;
            result.Name = prop.Name;
            result.IsStaticFromEntityFramework = true;
            result.IsSystem = true;
            if (context.GetField("Schema") == null)
                throw new Exception("This context does not have the Schema field, which stores the schema name");
            result.Schema = context.GetField("Schema").GetValue(context).ToString();
            result.Description = $"System {prop.Name} entity";
            var entityProps = entity.GetProperties().Select(x => x.Name).Except(baseProps).ToList();
            foreach (var field in entityProps)
            {
                var propField = entity.GetProperties().FirstOrDefault(x => x.Name.Equals(field));
                if (propField == null) continue;
                var propType = propField.PropertyType.FullName;
                switch (propType)
                {
                    case "System.String":
                        fields.Add(new CreateTableFieldViewModel
                        {
                            Name = field,
                            DisplayName = field,
                            TableFieldCode = "10",
                            DataType = TableFieldDataType.String,
                            Configurations = new List<FieldConfigViewModel>
                            {
                                new FieldConfigViewModel
                                {
                                    ConfigCode = "1000",
                                    Value = "500"
                                }
                            }
                        });
                        break;

                    case "System.Guid":
                        var f = new CreateTableFieldViewModel
                        {
                            Name = field,
                            DisplayName = field,
                            TableFieldCode = "30",
                            DataType = TableFieldDataType.Guid
                        };

                        if (field.EndsWith("Id"))
                        {
                            var ent = field.Remove(field.Length - "Id".Length);
                            if (entityProps.Contains(ent))
                            {
                                f.Parameter = "EntityReference";
                                f.Configurations = new List<FieldConfigViewModel>
                                {
                                    new FieldConfigViewModel
                                    {
                                        ConfigCode = TableFieldConfigCode.Reference.ForeingSchemaTable,
                                        Value = result.Schema
                                    },
                                    new FieldConfigViewModel
                                    {
                                        ConfigCode = TableFieldConfigCode.Reference.ForeingTable,
                                        Value = ent
                                    }
                                };
                            }
                        }

                        fields.Add(f);
                        break;
                    case "System.Int":
                    case "System.Int32":
                        fields.Add(new CreateTableFieldViewModel
                        {
                            Name = field,
                            DisplayName = field,
                            TableFieldCode = "01",
                            DataType = TableFieldDataType.Int,
                            Configurations = new List<FieldConfigViewModel>
                            {
                                new FieldConfigViewModel
                                {
                                    ConfigCode = "0100",
                                    Value = null
                                },
                                new FieldConfigViewModel
                                {
                                    ConfigCode = "0101",
                                    Value = null
                                }
                            }
                        });
                        break;
                    case "System.Double":
                        fields.Add(new CreateTableFieldViewModel
                        {
                            Name = field,
                            DisplayName = field,
                            TableFieldCode = "01",
                            DataType = TableFieldDataType.Int,
                            Configurations = new List<FieldConfigViewModel>
                            {
                                new FieldConfigViewModel
                                {
                                    ConfigCode = "0100",
                                    Value = null
                                },
                                new FieldConfigViewModel
                                {
                                    ConfigCode = "0101",
                                    Value = null
                                }
                            }
                        });
                        break;
                    case "System.Boolean":
                        fields.Add(new CreateTableFieldViewModel
                        {
                            Name = field,
                            DisplayName = field,
                            TableFieldCode = "40",
                            DataType = TableFieldDataType.Boolean,
                            Configurations = new List<FieldConfigViewModel>
                            {
                                new FieldConfigViewModel
                                {
                                    ConfigCode = "4000",
                                    Value = null
                                }
                            }
                        });
                        break;
                }
            }
            result.Fields = fields;
            return result;
        }
    }
}
