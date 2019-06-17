using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Query;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;

namespace ST.Entities.Data
{
    public static class DbContextExtension
    {
        private static IEntityQueryBuilder QueryBuilder => IoC.Resolve<IEntityQueryBuilder>();

        public static ResultModel<EntityViewModel> GetEntityByParams(this EntitiesDbContext dbContext,
            EntityViewModel viewModel)
        {

            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false,
                Result = new EntityViewModel { Includes = new List<EntityViewModel>() }
            };

            //Create Default Field
            //viewModel = ViewModelBuilder.Create(dbContext, viewModel);

            //For Add Single Field To Parent if no Exist
            viewModel = AddFieldForSingle(viewModel);

            returnModel.Result = viewModel;

            if (viewModel == null) return returnModel;

            try
            {
                var finalResult = new List<Dictionary<string, object>>();

                if (viewModel.Values != null && viewModel.Values.Count > 0)
                {
                    foreach (var listTableValue in viewModel.Values)
                    {
                        var sqlQuery = QueryBuilder.GetByColumnParameterQuery(viewModel, listTableValue);

                        var result = EntitiesFromSql(dbContext, sqlQuery, listTableValue).ToList();
                        finalResult.AddRange(result);
                    }
                }
                else
                {
                    var parameters = new Dictionary<string, object>();
                    var sqlQuery = QueryBuilder.GetByColumnParameterQuery(viewModel, parameters);
                    var result = EntitiesFromSql(dbContext, sqlQuery, parameters).ToList();
                    finalResult.AddRange(result);
                }

                returnModel.Result.Values = finalResult;
                returnModel.IsSuccess = true;
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }

            var values = GetRecursiveSingle(dbContext, viewModel, returnModel.Result.Values);
            returnModel.Result.Values = values;
            return returnModel;
        }

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="viewModel"></param>
        /// <param name="parameterId"></param>
        /// <returns></returns>
        public static ResultModel<EntityViewModel> GetEntityById(this EntitiesDbContext dbContext,
            EntityViewModel viewModel, Guid parameterId)
        {
            viewModel.Values =
                new List<Dictionary<string, object>> { { new Dictionary<string, object> { { "Id", parameterId } } } };
            return GetEntityByParams(dbContext, viewModel);
        }

        /// <summary>
        /// Insert new row in database
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static ResultModel<Guid> Insert(this EntitiesDbContext dbContext, EntityViewModel viewModel)
        {
            var returnModel = new ResultModel<Guid>
            {
                IsSuccess = false
            };
            if (viewModel == null) return returnModel;
            try
            {
                viewModel = SetDefaultInsertValues(viewModel);
                foreach (var value in viewModel.Values)
                {
                    var sqlQuery = QueryBuilder.InsertQuery(viewModel);

                    if (string.IsNullOrEmpty(sqlQuery)) continue;

                    using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;

                        if (cmd.Connection.State != ConnectionState.Open)
                            cmd.Connection.Open();

                        if (dbContext.Database.CurrentTransaction != null)
                            cmd.Transaction = ((RelationalTransaction)dbContext.Database.CurrentTransaction)
                                .GetDbTransaction();

                        foreach (var item in viewModel.Fields)
                        {
                            var dbParameter = cmd.CreateParameter();
                            dbParameter.ParameterName = string.Format("@{0}", item.ColumnName);
                            if (value[item.ColumnName] is Guid val)
                            {
                                if (val == Guid.Empty)
                                {
                                    dbParameter.Value = DBNull.Value;
                                }
                                else
                                {
                                    dbParameter.Value = value[item.ColumnName] ?? DBNull.Value;
                                }
                            }
                            else
                            {
                                dbParameter.Value = value[item.ColumnName] ?? DBNull.Value;
                            }

                            cmd.Parameters.Add(dbParameter);
                        }

                        cmd.ExecuteNonQuery();
                    }

                    returnModel.IsSuccess = true;
                    returnModel.Result = Guid.Parse(value["Id"].ToString());
                    return returnModel;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                returnModel.IsSuccess = false;
                returnModel.Errors.Add(new ErrorModel(ex.StackTrace, ex.Message));
                return returnModel;
            }

            return returnModel;
        }

        public static ResultModel<bool> Refresh(this EntitiesDbContext dbContext, EntityViewModel viewModel)
        {
            var queryBuilder = IoC.Resolve<IEntityQueryBuilder>();
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            if (viewModel == null) return returnModel;
            try
            {
                foreach (var value in viewModel.Values)
                {
                    var sqlQuery = queryBuilder.UpdateQuery(viewModel);

                    if (string.IsNullOrEmpty(sqlQuery)) continue;
                    using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;
                        if (cmd.Connection.State != ConnectionState.Open)
                            cmd.Connection.Open();

                        if (dbContext.Database.CurrentTransaction != null)
                            cmd.Transaction = ((RelationalTransaction)dbContext.Database.CurrentTransaction)
                                .GetDbTransaction();

                        foreach (var item in viewModel.Fields)
                        {
                            if (value[item.ColumnName] is Guid g)
                            {
                                if (g == Guid.Empty)
                                {
                                    value[item.ColumnName] = null;
                                }
                            }

                            var dbParameter = cmd.CreateParameter();
                            dbParameter.ParameterName = string.Format("@{0}", item.ColumnName);
                            dbParameter.Value = value[item.ColumnName] ?? DBNull.Value;
                            cmd.Parameters.Add(dbParameter);
                        }

                        cmd.ExecuteNonQuery();
                    }

                    returnModel.IsSuccess = true;
                    returnModel.Result = true;
                    return returnModel;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return returnModel;
            }

            return returnModel;
        }

        /// <summary>
        /// Get data by params
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static ResultModel<EntityViewModel> ListEntitiesByParams(this EntitiesDbContext dbContext,
            EntityViewModel viewModel)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false,
                Result = new EntityViewModel { Includes = new List<EntityViewModel>() }
            };

            //For Add Single Field To Parent if no Exist
            viewModel = AddFieldForSingle(viewModel);

            returnModel.Result = viewModel;

            if (viewModel == null) return returnModel;

            try
            {
                var finalResult = new List<Dictionary<string, object>>();

                if (viewModel.Values != null && viewModel.Values.Count > 0)
                {
                    foreach (var listTableValue in viewModel.Values)
                    {
                        var sqlQuery = QueryBuilder.GetByColumnParameterQuery(viewModel, listTableValue);

                        var result = EntitiesFromSql(dbContext, sqlQuery, listTableValue).ToList();
                        finalResult.AddRange(result);
                    }
                }
                else
                {
                    var parameters = new Dictionary<string, object>();
                    var sqlQuery = QueryBuilder.GetByColumnParameterQuery(viewModel, parameters);
                    var result = EntitiesFromSql(dbContext, sqlQuery, parameters).ToList();
                    finalResult.AddRange(result);
                }

                returnModel.Result.Values = finalResult;
                returnModel.IsSuccess = true;
            }
            catch (Exception ex)
            {
                returnModel.Errors = new List<IErrorModel>
                {
                    new ErrorModel("_ex", ex.ToString())
                };
                return returnModel;
            }

            var values = GetRecursiveSingle(dbContext, viewModel, returnModel.Result.Values);
            returnModel.Result.Values = values;
            return returnModel;
        }

        /// <summary>
        /// Delete row by id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="viewModel"></param>
        /// <param name="completeDelete"></param>
        /// <returns></returns>
        public static ResultModel<bool> DeleteById(this DbContext dbContext, EntityViewModel viewModel,
            bool completeDelete = false)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false,
                Result = false
            };
            if (viewModel == null) return returnModel;
            try
            {
                foreach (var value in viewModel.Values)
                {
                    var sqlQuery = QueryBuilder.DeleteByIdQuery(viewModel, completeDelete) ??
                                   throw new ArgumentNullException(
                                       $"{nameof(QueryBuilder.DeleteByIdQuery)}");

                    //var sqlQuery = EntityQueryBuilder.DeleteByIdQuery(viewModel.TableName, (Guid)value["Id"]);
                    if (string.IsNullOrEmpty(sqlQuery)) continue;
                    using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;
                        if (cmd.Connection.State != ConnectionState.Open)
                            cmd.Connection.Open();

                        if (dbContext.Database.CurrentTransaction != null)
                            cmd.Transaction = ((RelationalTransaction)dbContext.Database.CurrentTransaction)
                                .GetDbTransaction();

                        //Added Parameter
                        var dbParameter = cmd.CreateParameter();
                        dbParameter.ParameterName = "@Id";
                        dbParameter.Value = (Guid)value["Id"];
                        cmd.Parameters.Add(dbParameter);

                        cmd.ExecuteNonQuery();
                    }

                    returnModel.IsSuccess = true;
                    returnModel.Result = true;
                    return returnModel;
                }
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }

            return returnModel;
        }

        /// <summary>
        /// Get count by params
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static ResultModel<EntityViewModel> GetCountByParameter(this EntitiesDbContext dbContext,
            EntityViewModel viewModel)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false,
                Result = null
            };
            if (viewModel == null) return returnModel;
            var finalResult = new List<Dictionary<string, object>>();

            try
            {
                foreach (var value in viewModel.Values)
                {
                    var sqlQuery = QueryBuilder.GetCountByParameter(viewModel, value);
                    var result = EntitiesFromSql(dbContext, sqlQuery, value).FirstOrDefault();
                    finalResult.Add(result);
                }

                viewModel.Values = finalResult;
                returnModel.Result = viewModel;
                returnModel.IsSuccess = true;
                return returnModel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return returnModel;
            }
        }

        /// <summary>
        /// Count all data in entity
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static ResultModel<int> GetCount(this EntitiesDbContext dbContext,
            EntityViewModel viewModel)
        {
            var returnModel = new ResultModel<int>
            {
                IsSuccess = false,
                Result = 0
            };
            if (viewModel == null) return returnModel;

            try
            {
                var sqlQuery = QueryBuilder.GetCountByParameter(viewModel, new Dictionary<string, object>());
                var result = EntitiesFromSql(dbContext, sqlQuery, new Dictionary<string, object>()).FirstOrDefault();
                var data = result?.FirstOrDefault();
                if (data.Equals(default(KeyValuePair<string, object>))) return returnModel;
                returnModel.Result = Convert.ToInt32(data?.Value);
                returnModel.IsSuccess = true;
                return returnModel;
            }
            catch (Exception)
            {
                return returnModel;
            }
        }

        /// <summary>
        /// Get entities from SQL
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static IEnumerable<Dictionary<string, object>> EntitiesFromSql(this EntitiesDbContext dbContext,
            string sql, Dictionary<string, object> parameters)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            var result = new List<Dictionary<string, object>>();
            using (var cmd = dbContext.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = sql;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                if (dbContext.Database.CurrentTransaction != null)
                    cmd.Transaction =
                        ((RelationalTransaction)dbContext.Database.CurrentTransaction).GetDbTransaction();

                foreach (var param in parameters)
                {
                    if (param.Value == null) continue;
                    var dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = string.Format("@{0}", param.Key);
                    dbParameter.Value = param.Value;

                    cmd.Parameters.Add(dbParameter);
                }

                try
                {
                    using (var dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            var dataRow = GetDataRow(dataReader);
                            result.Add(dataRow);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return result;
            }
        }


        //Denis
        //To Delete 


        private static List<Dictionary<string, object>> GetRecursiveSingle(this EntitiesDbContext dbContext,
            EntityViewModel entityViewModel, List<Dictionary<string, object>> values)
        {
            //Every Table in Include
            if (entityViewModel.Includes == null) return values;

            foreach (var includeTable in entityViewModel.Includes)
            {
                //Add Default Fields 
                //entityViewModel = ViewModelBuilder.Create(dbContext, includeTable);

                var fieldNameForReturn = includeTable.Fields.FirstOrDefault(x => x.Type == "EntityName").ColumnName;
                // Check Every field in included Table
                foreach (var field in includeTable.Fields)
                {
                    if (field.Type != "Single" && field.Type != "Multiple") continue;

                    //Foreach entities from dataTable
                    foreach (var entities in values)
                    {
                        var valueTable = includeTable.Values;

                        // For Single == field.ColumnName for Multiple == id
                        var fieldData = field.Type == "Single"
                            ? entities.FirstOrDefault(x => x.Key == field.ColumnName).Value
                            : entities.FirstOrDefault(x => x.Key == "Id").Value;

                        if (fieldData == null) continue;

                        if (valueTable != null)
                        {
                            foreach (var value in valueTable)
                            {
                                try
                                {
                                    var parameters = value;
                                    parameters.Add(field.Type == "Single" ? "Id" : field.ColumnName, fieldData);

                                    //Changed GetBytColumnParameterQuery for only visible field non Single or Multiple
                                    var sqlQuery = QueryBuilder.GetByColumnParameterQuery(includeTable, parameters);

                                    var result = EntitiesFromSql(dbContext, sqlQuery, parameters).ToList();

                                    var tempData = GetRecursiveSingle(dbContext, includeTable, result);

                                    if (field.Type == "Single")
                                    {
                                        entities.Add(
                                            !string.IsNullOrEmpty(fieldNameForReturn)
                                                ? fieldNameForReturn
                                                : field.ColumnName.Replace("Id", string.Empty),
                                            tempData.FirstOrDefault() ?? null);
                                    }
                                    else
                                    {
                                        entities.Add(
                                            !string.IsNullOrEmpty(fieldNameForReturn)
                                                ? fieldNameForReturn
                                                : field.ColumnName.Replace("Id", "s"), tempData ?? null);
                                    }
                                }
                                catch (Exception)
                                {
                                    //Error
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                var parameters = new Dictionary<string, object>
                                {
                                    {field.Type == "Single" ? "Id" : field.ColumnName, fieldData}
                                };

                                //Changed GetBytColumnParameterQuery for only visible field non Single or Multiple
                                var sqlQuery = QueryBuilder.GetByColumnParameterQuery(includeTable, parameters);

                                var result = EntitiesFromSql(dbContext, sqlQuery, parameters).ToList();

                                var tempData = GetRecursiveSingle(dbContext, includeTable, result);

                                if (field.Type == "Single")
                                {
                                    entities.Add(
                                        !string.IsNullOrEmpty(fieldNameForReturn)
                                            ? fieldNameForReturn
                                            : field.ColumnName.Replace("Id", string.Empty),
                                        tempData.FirstOrDefault() ?? null);
                                }
                                else
                                {
                                    entities.Add(
                                        !string.IsNullOrEmpty(fieldNameForReturn)
                                            ? fieldNameForReturn
                                            : field.ColumnName.Replace("Id", "s"), tempData ?? null);
                                }
                            }
                            catch (Exception)
                            {
                                //Error
                            }
                        }
                    }
                }
            }

            return values;
        }

        //Check and add if no exist "Single" Field in Parent EntityViewModel
        private static EntityViewModel AddFieldForSingle(EntityViewModel entityViewModel)
        {
            if (entityViewModel.Includes == null || entityViewModel.Includes.Count == 0)
            {
                return entityViewModel;
            }

            foreach (var model in entityViewModel.Includes)
            {
                var temp = model.Fields.Where(x => x.Type == "Single").Select(x => x.ColumnName);
                foreach (var fieldTemp in temp)
                    if (entityViewModel.Fields.All(x => x.ColumnName != fieldTemp))
                        entityViewModel.Fields.Add(new EntityFieldsViewModel { ColumnName = fieldTemp });
            }

            for (var i = 0; i < entityViewModel.Includes.Count; i++)
            {
                entityViewModel.Includes[i] = AddFieldForSingle(entityViewModel.Includes[i]);
            }

            return entityViewModel;
        }

        public static ResultModel<EntityViewModel> ListEntitiesByParamsRecursive(this EntitiesDbContext dbContext,
            EntityViewModel entityModel)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false,
                Result = new EntityViewModel { Includes = new List<EntityViewModel>() }
            };


            //Create Default Field
            //entityModel = ViewModelBuilder.Create(dbContext, entityModel);

            //For Add Single Field To Parent if no Exist
            entityModel = AddFieldForSingle(entityModel);

            returnModel.Result = entityModel;

            if (entityModel == null) return returnModel;

            try
            {
                var finalResult = new List<Dictionary<string, object>>();

                if (entityModel.Values != null && entityModel.Values.Count > 0)
                {
                    foreach (var listTableValue in entityModel.Values)
                    {
                        var sqlQuery = QueryBuilder.GetByColumnParameterQuery(entityModel, listTableValue) ??
                                       throw new ArgumentNullException(
                                           $"{nameof(QueryBuilder.GetByColumnParameterQuery)}");
                        var result = EntitiesFromSql(dbContext, sqlQuery, listTableValue).ToList();
                        finalResult.AddRange(result);
                    }
                }
                else
                {
                    var parameters = new Dictionary<string, object>();
                    var sqlQuery = QueryBuilder.GetByColumnParameterQuery(entityModel, parameters);
                    var result = EntitiesFromSql(dbContext, sqlQuery, parameters).ToList();
                    finalResult.AddRange(result);
                }

                returnModel.Result.Values = finalResult;
                returnModel.IsSuccess = true;
            }
            catch (Exception)
            {
                // Error
                return returnModel;
            }

            var values = GetRecursiveSingle(dbContext, entityModel, returnModel.Result.Values);
            returnModel.Result.Values = values;
            return returnModel;
        }

        /// <summary>
        /// Get data row
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private static Dictionary<string, object> GetDataRow(IDataRecord dataReader)
        {
            var dataRow = new Dictionary<string, object>();
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            return dataRow;
        }

        /// <summary>
        /// Set default insert values
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private static EntityViewModel SetDefaultInsertValues(EntityViewModel viewModel)
        {
            if (viewModel.Fields.All(s => s.ColumnName != "Id"))
            {
                viewModel.Fields.Add(new EntityFieldsViewModel
                {
                    IsSystem = true,
                    ColumnName = "Id"
                });
            }

            if (viewModel.Fields.All(s => s.ColumnName != "Created"))
            {
                viewModel.Fields.Add(new EntityFieldsViewModel
                {
                    IsSystem = true,
                    ColumnName = "Created"
                });
            }

            foreach (var value in viewModel.Values)
            {
                if (!value.ContainsKey("Id"))
                {
                    value.Add("Id", Guid.NewGuid());
                }
                else if (value.ContainsKey("Id"))
                {
                    try
                    {
                        if (Guid.Parse(value["Id"].ToString()) == Guid.Empty) value["Id"] = Guid.NewGuid();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                if (!value.ContainsKey("Created"))
                {
                    value.Add("Created", DateTime.Now);
                }

                if (value.ContainsKey("Created"))
                {
                    value["Created"] = DateTime.Now;
                }

                if (value.ContainsKey("Changed"))
                {
                    value["Changed"] = DateTime.Now;
                }
                else
                {
                    value.Add("Changed", DateTime.Now);
                }

                if (value.ContainsKey("IsDeleted"))
                {
                    value["IsDeleted"] = false;
                }
                else
                {
                    value.Add("IsDeleted", false);
                }
            }

            return viewModel;
        }
    }
}