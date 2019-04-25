using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ST.BaseBusinessRepository;
using ST.Entities.Controls.Querry;
using ST.Entities.Models.Tables;
using ST.Entities.Services.Abstraction;
using ST.Entities.Settings;
using ST.Entities.ViewModels.Table;
using ST.Shared;

namespace ST.Entities.Services
{
    public class TablesService : ITablesService
    {
        /// <inheritdoc />
        /// <summary>
        /// Add field SQL
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <param name="connectionString"></param>
        /// <param name="isNew"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> AddFieldSql(CreateTableFieldViewModel table, string tableName, string connectionString,
            bool isNew, string tableSchema)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            var sqlQuery = isNew
                ? TableQuerryBuilder.AddFieldQuerry(table, tableName, tableSchema)
                : TableQuerryBuilder.UpdateFieldQuerry(table, tableName, tableSchema);
            if (!string.IsNullOrEmpty(sqlQuery))
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                returnModel.IsSuccess = true;
                returnModel.Result = true;
                return returnModel;
            }

            // Empty query
            return returnModel;
        }

        /// <inheritdoc />
        /// <summary>
        /// Check column values
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> CheckColumnValues(string connectionString, string tableName, string tableSchema, string columnName)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    var sqlQuery = TableQuerryBuilder.CheckColumnValues(tableName, columnName, tableSchema);
                    using (var connection = new SqlConnection(connectionString))
                    {
                        var command = new SqlCommand(sqlQuery, connection);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.HasRows)
                                {
                                    returnModel.Result = true;
                                }
                            }
                        }
                    }

                    return returnModel;
                }
                catch (Exception)
                {
                    return returnModel;
                }
            }

            return returnModel;
        }

        /// <inheritdoc />
        /// <summary>
        /// Check table values
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> CheckTableValues(string connectionString, string tableName, string tableSchema)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    var sqlQuery = TableQuerryBuilder.CheckTableValues(tableName, tableSchema);
                    using (var connection = new SqlConnection(connectionString))
                    {
                        var command = new SqlCommand(sqlQuery, connection);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.HasRows)
                                {
                                    returnModel.Result = true;
                                }
                            }
                        }
                    }

                    return returnModel;
                }
                catch (Exception)
                {
                    return returnModel;
                }
            }
            else
            {
                return returnModel;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Create SQL table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> CreateSqlTable(TableModel table, string connectionString)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = true
            };

            var schemas = GetSchemas(connectionString);

            if (!schemas.Contains(table.EntityType))
            {
                CreateSchemaAsync(table.EntityType, connectionString).GetAwaiter().GetResult();
            }

            try
            {
                var sqlQuery = TableQuerryBuilder.CreateQuerry(table);
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        var command = new SqlCommand(sqlQuery, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    returnModel.IsSuccess = true;
                    return returnModel;
                }
                else
                {
                    // Empty query
                    return returnModel;
                }
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Drop column
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> DropColumn(string connectionString, string tableName, string tableSchema, string columnName)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            try
            {
                var sqlQuery = TableQuerryBuilder.DropColumn(tableName, tableSchema, columnName);
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        var command = new SqlCommand(sqlQuery, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    returnModel.Result = true;
                    return returnModel;
                }
                else
                {
                    // Empty query
                    return returnModel;
                }
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }
        }

        /// <summary>
        /// Get static entities from contexts
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns></returns>
        public static IEnumerable<SynchronizeTableViewModel> GetEntitiesFromDbContexts(params Type[] contexts)
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
        private static IEnumerable<SynchronizeTableViewModel> GetEntitiesFromContext(Type context)
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
        private static SynchronizeTableViewModel GetEntityData(PropertyInfo prop, Type context)
        {
            var result = new SynchronizeTableViewModel();
            var fields = new List<CreateTableFieldViewModel>();
            var baseProps = typeof(ExtendedModel).GetProperties().Select(x => x.Name);
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
                                        ConfigCode = "9999",
                                        Value = result.Schema
                                    },
                                    new FieldConfigViewModel
                                    {
                                        ConfigCode = "3000",
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

        /// <inheritdoc />
        /// <summary>
        /// Drop constraint
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <param name="constraint"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> DropConstraint(string connectionString, string tableName, string tableSchema, string constraint,
            string columnName)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            try
            {
                var sqlQuery = TableQuerryBuilder.DropConstraint(tableName, tableSchema, constraint, columnName);
                if (string.IsNullOrEmpty(sqlQuery)) return returnModel;
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(sqlQuery, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                returnModel.Result = true;
                return returnModel;

                // Empty query
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Drop table
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public virtual ResultModel<bool> DropTable(string connectionString, string tableName, string tableSchema)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            try
            {
                var check = false;
                var sqlCheckQuery = TableQuerryBuilder.CheckTableValues(tableName, tableSchema);
                using (var connection = new SqlConnection(connectionString))
                {
                    var command = new SqlCommand(sqlCheckQuery, connection);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.HasRows)
                            {
                                check = true;
                            }
                        }
                    }
                }

                if (check) return returnModel;
                {
                    var sqlQuery = TableQuerryBuilder.DropTable(tableName, tableSchema);
                    if (string.IsNullOrEmpty(sqlQuery)) return returnModel;
                    using (var connection = new SqlConnection(connectionString))
                    {
                        var command = new SqlCommand(sqlQuery, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    returnModel.Result = true;
                    return returnModel;
                }
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }
        }
        /// <summary>
        /// Create Schema
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="connectionString"></param>
        protected virtual async Task CreateSchemaAsync(string schemaName, string connectionString)
        {
            var all = GetSchemas(connectionString);
            if (all.Contains(schemaName)) return;
            var sqlQuery = TableQuerryBuilder.GetSchemaSqlScript(schemaName);
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }
        }

        /// <summary>
        /// Get all schemas
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetSchemas(string connectionString)
        {
            var result = new List<string>();
            var sqlQuery = TableQuerryBuilder.GetDbSchemesSqlScript();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(sqlQuery, connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            result.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return result;
        }
    }
}