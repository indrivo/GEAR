using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.Query;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.EntityBuilder.MsSql
{
    /// <inheritdoc />
    public class MsSqlTablesService : TablesService
    {
        private static readonly IQueryTableBuilder QueryTableBuilder = IoC.Resolve<IQueryTableBuilder>();

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
        public override ResultModel<bool> AddFieldSql(CreateTableFieldViewModel table, string tableName, string connectionString,
            bool isNew, string tableSchema)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            var sqlQuery = isNew
                ? QueryTableBuilder.AddFieldQuery(table, tableName, tableSchema)
                : QueryTableBuilder.UpdateFieldQuery(table, tableName, tableSchema);
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
        public override ResultModel<bool> CheckColumnValues(string connectionString, string tableName, string tableSchema, string columnName)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            if (string.IsNullOrEmpty(connectionString)) return returnModel;
            try
            {
                var sqlQuery = QueryTableBuilder.CheckColumnValues(tableName, columnName, tableSchema);
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

        /// <inheritdoc />
        /// <summary>
        /// Check table values
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public override ResultModel<bool> CheckTableValues(string connectionString, string tableName, string tableSchema)
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
                    var sqlQuery = QueryTableBuilder.CheckTableValues(tableName, tableSchema);
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
        public override ResultModel<bool> CreateSqlTable(TableModel table, string connectionString)
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
                var sqlQuery = QueryTableBuilder.CreateQuery(table);
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
        public override ResultModel<bool> DropColumn(string connectionString, string tableName, string tableSchema, string columnName)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            try
            {
                var sqlQuery = QueryTableBuilder.DropColumn(tableName, tableSchema, columnName);
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
        public override ResultModel<bool> DropConstraint(string connectionString, string tableName, string tableSchema, string constraint,
            string columnName)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            try
            {
                var sqlQuery = QueryTableBuilder.DropConstraint(tableName, tableSchema, constraint, columnName);
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
        public override ResultModel<bool> DropTable(string connectionString, string tableName, string tableSchema)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            try
            {
                var check = false;
                var sqlCheckQuery = QueryTableBuilder.CheckTableValues(tableName, tableSchema);
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
                    var sqlQuery = QueryTableBuilder.DropTable(tableName, tableSchema);
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
        public override async Task CreateSchemaAsync(string schemaName, string connectionString)
        {
            var all = GetSchemas(connectionString);
            if (all.Contains(schemaName)) return;
            var sqlQuery = QueryTableBuilder.GetSchemaSqlScript(schemaName);
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
        public override IEnumerable<string> GetSchemas(string connectionString)
        {
            var result = new List<string>();
            var sqlQuery = QueryTableBuilder.GetDbSchemesSqlScript();
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