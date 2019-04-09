using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using ST.BaseBusinessRepository;
using ST.Entities.Controls.Querry;
using ST.Entities.Models.Tables;
using ST.Entities.ViewModels.Table;

namespace ST.Entities.Services
{
    public class NpgTablesService : TablesService
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
        public override ResultModel<bool> AddFieldSql(CreateTableFieldViewModel table, string tableName, string connectionString,
            bool isNew, string tableSchema)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false,
                Errors = new List<IErrorModel>()
            };
            var sqlQuery = isNew
                ? NpgTableQuerryBuilder.AddFieldQuerry(table, tableName, tableSchema)
                : NpgTableQuerryBuilder.UpdateFieldQuerry(table, tableName, tableSchema);
            if (string.IsNullOrEmpty(sqlQuery)) return returnModel;
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    var command = new NpgsqlCommand(sqlQuery, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                returnModel.IsSuccess = true;
                returnModel.Result = true;
                return returnModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                returnModel.Errors.Add(new ErrorModel("Exception", ex.ToString()));
                return returnModel;
            }
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
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    var sqlQuery = NpgTableQuerryBuilder.CheckColumnValues(tableName, tableSchema, columnName);
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        var command = new NpgsqlCommand(sqlQuery, connection);
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
                    var sqlQuery = NpgTableQuerryBuilder.CheckTableValues(tableName, tableSchema);
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        var command = new NpgsqlCommand(sqlQuery, connection);
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

                var sqlQuery = NpgTableQuerryBuilder.CreateQuerry(table);
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        var command = new NpgsqlCommand(sqlQuery, connection);
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
                var sqlQuery = NpgTableQuerryBuilder.DropColumn(tableName, columnName, tableSchema);
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        var command = new NpgsqlCommand(sqlQuery, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    returnModel.Result = true;
                    return returnModel;
                }

                returnModel.Errors = new List<IErrorModel>
                {
                    new ErrorModel(Guid.NewGuid().ToString(), "Empty query!")
                };
                return returnModel;
            }
            catch (Exception ex)
            {
                returnModel.Errors = new List<IErrorModel>
                {
                    new ErrorModel(Guid.NewGuid().ToString(),ex.ToString())
                };
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
                var sqlQuery = NpgTableQuerryBuilder.DropConstraint(tableName, constraint, columnName, tableSchema);
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        var command = new NpgsqlCommand(sqlQuery, connection);
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
                var sqlCheckQuerry = NpgTableQuerryBuilder.CheckTableValues(tableName, tableSchema);
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    var command = new NpgsqlCommand(sqlCheckQuerry, connection);
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
                    var sqlQuery = NpgTableQuerryBuilder.DropTable(tableName, tableSchema);
                    if (string.IsNullOrEmpty(sqlQuery)) return returnModel;
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        var command = new NpgsqlCommand(sqlQuery, connection);
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

        /// <inheritdoc />
        /// <summary>
        /// Create Schema
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="connectionString"></param>
        protected override async Task CreateSchemaAsync(string schemaName, string connectionString)
        {
            var all = GetSchemas(connectionString);
            if (all.Contains(schemaName)) return;
            var sqlQuery = NpgTableQuerryBuilder.GetSchemaSqlScript(schemaName);
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var command = new NpgsqlCommand(sqlQuery, connection);
                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all schemas
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected override IEnumerable<string> GetSchemas(string connectionString)
        {
            var result = new List<string>();
            var sqlQuerry = NpgTableQuerryBuilder.GetDbSchemesSqlScript();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                var command = new NpgsqlCommand(sqlQuerry, connection);
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