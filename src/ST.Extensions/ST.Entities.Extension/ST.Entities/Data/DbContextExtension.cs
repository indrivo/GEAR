using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Events;
using ST.Entities.Abstractions.Events.EventArgs;
using ST.Entities.Abstractions.Helpers;
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
            var watch = new Stopwatch();
            var evArgs = new ExecutedQueryEventArgs();
            watch.Start();
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
                        evArgs.Query = sqlQuery;
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
                evArgs.Exception = ex;
                evArgs.Completed = false;
                watch.Stop();
                evArgs.Elapsed = watch.ElapsedMilliseconds;
                EntityEvents.SqlQuery.QueryExecuted(evArgs);
                Debug.WriteLine(ex);
                return returnModel;
            }

            var values = GetRecursiveSingle(dbContext, viewModel, returnModel.Result.Values);
            returnModel.Result.Values = values;
            watch.Stop();
            evArgs.Elapsed = watch.ElapsedMilliseconds;
            EntityEvents.SqlQuery.QueryExecuted(evArgs);
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

                    using (var cmd = DbConnectionFactory.Connection.Get().CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;

                        if (dbContext.Database.CurrentTransaction != null)
                            cmd.Transaction = ((RelationalTransaction)dbContext.Database.CurrentTransaction)
                                .GetDbTransaction();

                        foreach (var item in viewModel.Fields)
                        {
                            var dbParameter = cmd.CreateParameter();
                            dbParameter.ParameterName = $"@{item.ColumnName}";
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
                    using (var cmd = DbConnectionFactory.Connection.Get().CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;

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
                            dbParameter.ParameterName = $"@{item.ColumnName}";
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
            var watch = new Stopwatch();
            watch.Start();
            var evArgs = new ExecutedQueryEventArgs();
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
                        evArgs.Query = sqlQuery;
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
                watch.Stop();
                evArgs.Elapsed = watch.ElapsedMilliseconds;
                evArgs.Completed = false;
                evArgs.Exception = ex;
                EntityEvents.SqlQuery.QueryExecuted(evArgs);
                returnModel.Errors.Add(new ErrorModel("_ex", ex.ToString()));
                return returnModel;
            }

            var values = GetRecursiveSingle(dbContext, viewModel, returnModel.Result.Values);
            returnModel.Result.Values = values;
            watch.Stop();
            evArgs.Elapsed = watch.ElapsedMilliseconds;
            EntityEvents.SqlQuery.QueryExecuted(evArgs);
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
                    using (var cmd = DbConnectionFactory.Connection.Get().CreateCommand())
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
            catch (Exception ex)
            {
                returnModel.Errors.Add(new ErrorModel(nameof(Exception), ex.Message));
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
        /// <param name="filters"></param>
        /// <returns></returns>
        public static ResultModel<int> GetCount(this EntitiesDbContext dbContext,
            EntityViewModel viewModel, Dictionary<string, object> filters = null)
        {
            var returnModel = new ResultModel<int>
            {
                IsSuccess = false,
                Result = 0
            };
            if (viewModel == null) return returnModel;

            try
            {
                var sqlQuery = QueryBuilder.GetCountByParameter(viewModel, filters);
                var result = EntitiesFromSql(dbContext, sqlQuery, filters).FirstOrDefault();
                var data = result?.FirstOrDefault();
                if (data.IsNull()) return returnModel;
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

            using (var cmd = DbConnectionFactory.Connection.Get().CreateCommand())
            {
                cmd.CommandText = sql;

                if (dbContext.Database.CurrentTransaction != null)
                    cmd.Transaction =
                        ((RelationalTransaction)dbContext.Database.CurrentTransaction).GetDbTransaction();

                foreach (var param in parameters)
                {
                    if (param.Value == null) continue;
                    var dbParameter = cmd.CreateParameter();
                    dbParameter.ParameterName = $"@{param.Key}";
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
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
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
            if (viewModel.Fields.All(s => s.ColumnName != nameof(BaseModel.Id)))
            {
                viewModel.Fields.Add(new EntityFieldsViewModel
                {
                    IsSystem = true,
                    ColumnName = nameof(BaseModel.Id)
                });
            }

            if (viewModel.Fields.All(s => s.ColumnName != nameof(BaseModel.Created)))
            {
                viewModel.Fields.Add(new EntityFieldsViewModel
                {
                    IsSystem = true,
                    ColumnName = "Created"
                });
            }

            foreach (var value in viewModel.Values)
            {
                if (!value.ContainsKey(nameof(BaseModel.Id)))
                {
                    value.Add(nameof(BaseModel.Id), Guid.NewGuid());
                }
                else if (value.ContainsKey(nameof(BaseModel.Id)))
                {
                    try
                    {
                        if (Guid.Parse(value[nameof(BaseModel.Id)].ToString()) == Guid.Empty) value[nameof(BaseModel.Id)] = Guid.NewGuid();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                if (!value.ContainsKey(nameof(BaseModel.Created)))
                {
                    value.Add(nameof(BaseModel.Created), DateTime.Now);
                }

                if (value.ContainsKey(nameof(BaseModel.Created)))
                {
                    value[nameof(BaseModel.Created)] = DateTime.Now;
                }

                if (value.ContainsKey(nameof(BaseModel.Changed)))
                {
                    value[nameof(BaseModel.Changed)] = DateTime.Now;
                }
                else
                {
                    value.Add(nameof(BaseModel.Changed), DateTime.Now);
                }

                if (value.ContainsKey(nameof(BaseModel.IsDeleted)))
                {
                    value[nameof(BaseModel.IsDeleted)] = false;
                }
                else
                {
                    value.Add(nameof(BaseModel.IsDeleted), false);
                }
            }

            return viewModel;
        }
    }
}