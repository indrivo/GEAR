using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ST.Core;
using ST.Core.Extensions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Report.Abstractions;
using ST.Report.Abstractions.Extensions;
using ST.Report.Abstractions.Models;
using ST.Report.Abstractions.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ST.Report.Dynamic
{
    public class DynamicReportsService<TContext> : IDynamicReportsService where TContext : DbContext, IReportContext
    {
        private readonly TContext _context;
        private readonly IConfiguration _configuration;
        private IHostingEnvironment HostingEnvironment { get; }

        public DynamicReportsService(TContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        private static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        private static DynamicReport ParseForDbModel(DynamicReport databaseReport, DynamicReport model)
        {
            databaseReport.Name = model.Name;
            return databaseReport;
        }

        #region Maps Region

        /// <summary>
        /// Create Map
        /// </summary>
        public void CreateFolder(DynamicReportFolder folder)
        {
            _context.DynamicReportsFolders.Add(folder);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get folder by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DynamicReportFolder GetFolder(Guid id)
        {
            return _context.DynamicReportsFolders.First(x => x.Id == id);
        }

        /// <summary>
        /// Edit Map
        /// </summary>
        /// <param name="newFolder"></param>
        public void EditFolder(DynamicReportFolder newFolder)
        {
            _context.DynamicReportsFolders.Update(newFolder);
            _context.SaveChanges();
        }

        /// <summary>
        /// Delete Map
        /// </summary>
        public void DeleteFolder(Guid id)
        {
            _context.DynamicReportsFolders.Remove(_context.DynamicReportsFolders.First(x => x.Id == id));
            _context.SaveChanges();
        }

        public IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReport>> GetAllFolders()
        {
            return _context.DynamicReportsFolders.Include(x => x.Reports);
        }

        #endregion

        #region Reports Region

        /// <summary>
        /// Get reports
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public DTResult<DynamicReport> GetFilteredReports(DTParameters param)
        {
            var filtered = _context.Filter<DynamicReport>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount);

            var finalResult = new DTResult<DynamicReport>
            {
                Draw = param.Draw,
                Data = filtered.Select(x =>
                {
                    var folder = _context.DynamicReportsFolders.FirstOrDefault(y => y.Id == x.DynamicReportFolderId);
                    var obj = x;
                    x.DynamicReportFolder = new DynamicReportFolder
                    {
                        Name = folder?.Name
                    };
                    return x;
                }).ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return finalResult;
        }


        /// <summary>
        /// Create Report
        /// </summary>
        /// <param name="reportModel"></param>
        public void SaveReport(DynamicReport report)
        {
            if (report.Id == Guid.Empty)
            {
                report.Id = Guid.NewGuid();
                _context.DynamicReports.Add(report);
            }
            else
            {
                var entity = _context.DynamicReports.First(x => x.Id == report.Id);
                entity.Name = report.Name;
                entity.ReportDataModel = report.ReportDataModel;
                entity.IsDeleted = report.IsDeleted;
                entity.ModifiedBy = report.ModifiedBy;
                entity.Changed = report.Changed;
                entity.DynamicReportFolderId = report.DynamicReportFolderId;
                _context.Update(entity);
            }
            _context.SaveChanges();
        }

        public DynamicReport GetReport(Guid id)
        {
            return _context.DynamicReports.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Delete Report
        /// </summary>
        /// <param name="id"></param>
        public void DeleteReport(Guid id)
        {
            _context.DynamicReports.Remove(_context.DynamicReports.First(x => x.Id == id));
            _context.SaveChanges();
        }

        #endregion

        #region Database Data Gathering Region

        /// <summary>
        /// Get all table names from DB
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTableNames()
        {
            if (_context == null) throw new ArgumentNullException(nameof(_context));
            var tableNames = new List<string>();

            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Distinct TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA LIKE 'system'";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                tableNames.Add(reader.GetString(0));
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return tableNames.ToList();
        }

        /// <summary>
        /// Get Schema for a specific table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetTableSchema(string tableName)
        {
            if (_context == null) throw new ArgumentNullException(nameof(_context));
            var tableSchema = "";
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TABLE_SCHEMA FROM information_schema.TABLES WHERE TABLE_NAME LIKE '" + tableName + "' AND TABLE_SCHEMA = 'system'";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                tableSchema = (reader.GetString(0));
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return tableSchema;
        }

        /// <summary>
        /// Get a list of column names from a specific table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTableColumns(string tableName)
        {
            //var props = typeof(TContext).GetProperties().FirstOrDefault(x => x.Name.Equals(tableName) &&
            //                                                                   x.PropertyType.IsGenericType && x.PropertyType.GetGenericArguments().Any())
            //    ?.PropertyType.GetGenericArguments()[0].GetProperties().Where(x => (!x.PropertyType.IsClass && !x.PropertyType.IsInterface) || x.PropertyType == typeof(string)).Select(x => x.Name).ToList();
            var query = $"SELECT column_name FROM information_schema.columns WHERE table_schema = 'system' AND table_name = '{tableName}'";
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                yield return (reader.GetString(0));
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Get the type of the Column
        /// used in adding filters
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public dynamic GetColumnType(string tableName, string columnName)
        {
            var props = typeof(TContext).GetProperties().FirstOrDefault(x => x.Name.Equals(tableName) &&
                                                                               x.PropertyType.IsGenericType
                                                                               && x.PropertyType.GetGenericArguments().Any())
                ?.PropertyType.GetGenericArguments()[0].GetProperties().First(x => x.Name == columnName).PropertyType.Name;
            return props;
        }

        /// <summary>
        /// Get Current ConnectionString
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            var connection = _configuration.GetSection("ConnectionStrings")
                .GetSection("PostgreSQL")
                .GetValue<string>("ConnectionString");
            return connection;
        }

        /// <summary>
        /// Get the the table for specific column
        /// If column is fk specific PrimaryTable name is returned
        /// If column is not Fk null is returned
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string GetPrimaryTableName(string columnName)
        {
            if (_context == null) throw new ArgumentNullException(nameof(_context));
            var tableName = "";
            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            "SELECT PK.TABLE_NAME, PK.TABLE_SCHEMA FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C " +
                            "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME INNER " +
                            "    JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME INNER " +
                            "   JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME INNER JOIN " +
                            "(SELECT i1.TABLE_NAME, i2.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON " +
                            "i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY') PT ON PT.TABLE_NAME = PK.TABLE_NAME " +
                            "Where CU.COLUMN_NAME = '" + columnName + "' AND PK.TABLE_SCHEMA = 'system'";

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    tableName = (reader.GetString(0));
                                }
                            }

                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                connection.Close();
            }
            return tableName;
        }

        public dynamic GetForeignKeySelectValues(string tableName, string columnName)
        {
            var possiblePrimaryTable = GetPrimaryTableName(columnName);
            if (string.IsNullOrEmpty(possiblePrimaryTable)) return default;
            var command =
                $"SELECT DISTINCT ({possiblePrimaryTable}.Id) , ({possiblePrimaryTable}.Name) FROM [{GetTableSchema(possiblePrimaryTable)}].{possiblePrimaryTable}";

            //TODO: switch this part with something else to check user
            if (columnName == "UserId")
            {
                command = $"SELECT DISTINCT ({possiblePrimaryTable}.Id) , ({possiblePrimaryTable}.UserName) FROM [{GetTableSchema(possiblePrimaryTable)}].{possiblePrimaryTable}";
            }
            var resultContent = new List<DynamicReportQueryResultViewModel>();

            var reportDynamicDataModels = new DynamicReportColumnDataModel[2];
            reportDynamicDataModels[0] = new DynamicReportColumnDataModel() { Prefix = "", DataColumn = columnName };
            reportDynamicDataModels[1] = new DynamicReportColumnDataModel() { Prefix = "", DataColumn = "Name" };


            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var sqlCommand = connection.CreateCommand())
                {
                    sqlCommand.CommandText = command;

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var item = new DynamicReportQueryResultViewModel();

                                var array = reportDynamicDataModels.Select(data => data.DataColumn).ToList();

                                if (array.Count < columnName.Split(",").Length - 1) array.Add("Total");

                                for (var i = 0; i < array.Count; i++)
                                {
                                    item.Columns.Add(new ResultValues()
                                    {
                                        Column = array[i],
                                        Value = reader.GetValue(i).ToString()
                                    });
                                }

                                resultContent.Add(item);
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return resultContent;

        }

        public IEnumerable<dynamic> GetReportContent(DynamicReportDataModel dto)
        {
            if (_context == null) throw new ArgumentNullException(nameof(_context));


            var resultContent = new List<DynamicReportQueryResultViewModel>();


            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var sqlCommand = connection.CreateCommand())
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("SELECT ");
                    if (dto.FieldsList.Count() == 0)
                    {
                        queryBuilder.Append(" * ");
                    }
                    else
                    {
                        foreach (var field in dto.FieldsList)
                        {
                            if (field.AggregateType != AggregateType.None)
                            {
                                queryBuilder.Append($" {field.AggregateType}(system.{field.FieldName}) {(string.IsNullOrEmpty(field.FieldAlias) ? "" : $" AS \"{field.FieldAlias}\"")}");
                            }
                            else
                            {
                                queryBuilder.Append($" system.{field.FieldName} {(string.IsNullOrEmpty(field.FieldAlias) ? "" : $" AS \"{field.FieldAlias}\"")}");
                            }

                            if (!field.Equals(dto.FieldsList.Last()))
                            {
                                queryBuilder.Append($",");
                            }
                        }
                    }

                    queryBuilder.Append(" FROM ");

                    foreach (var table in dto.Tables)
                    {
                        if (table.Equals(dto.Tables.First()))
                        {
                            queryBuilder.Append($@" system.""{table}"" ");
                        }
                        else
                        {
                            queryBuilder.Append($@" INNER JOIN system.""{table}"" ");
                            var rel = dto.Relations.FirstOrDefault(s => s.ForeignKeyTable == table);
                            if (rel != null)
                            {
                                queryBuilder.Append($@" ON(system.""{rel.PrimaryKeyTable}"".""Id"" = system.""{rel.ForeignKeyTable}"".""{rel.ForeignKey}"") ");
                            }
                        }

                    }

                    var filterFields = dto.FiltersList.Where(s => s.FilterType != FilterType.GroupBy);

                    if (filterFields != null)
                    {
                        foreach (var filter in filterFields)
                        {
                            if (filter.Equals(filterFields.First()))
                            {
                                queryBuilder.Append($@" WHERE system.{filter.FieldName} {filter.FilterType.GetDisplayName()} {filter.Value} ");
                            }
                            else
                            {
                                queryBuilder.Append($@" AND system.{filter.FieldName} {filter.FilterType.GetDisplayName()} {filter.Value} ");
                            }
                        }
                    }

                    var groupByFields = dto.FiltersList.Where(s => s.FilterType == FilterType.GroupBy);

                    if (groupByFields != null)
                    {
                        foreach (var group in groupByFields)
                        {
                            if (group.Equals(groupByFields.First()))
                            {
                                queryBuilder.Append($@" GROUP BY system.{group.FieldName} ");
                            }
                            else
                            {
                                queryBuilder.Append($@", system.{group.FieldName} ");
                            }
                        }
                    }

                    sqlCommand.CommandText = queryBuilder.ToString();

                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return SqlDataReaderToExpando(reader);
                        }
                    }
                }
                connection.Close();
            }
        }


        private dynamic SqlDataReaderToExpando(NpgsqlDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            short duplicates = 0;
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (expandoObject.ContainsKey(reader.GetName(i)))
                {
                    expandoObject.Add($"{reader.GetName(i)}_{duplicates}", reader[i]);
                    duplicates++;
                }
                else
                {
                    expandoObject.Add(reader.GetName(i), reader[i]);
                }
            }
            return expandoObject;
        }


        #endregion
    }
}