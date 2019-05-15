using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ST.Core;
using ST.Report.Abstractions;
using ST.Report.Abstractions.Models;
using ST.Core.Extensions;
using ST.DynamicEntityStorage.Abstractions.Extensions;

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

        private static DynamicReportDbModel ParseForDbModel(DynamicReportDbModel databaseReport, DynamicReport model)
        {
            var columnListString = model.ColumnList.Aggregate("", (current, column)
                => current + (column.Prefix + "(" + column.DataColumn + ")|"));
            var filtersListString = model.Filters.Aggregate("", (current, filter)
                => current + (filter.FilterType + ":" + filter.ColumnName + ":" + filter.Operation + ":" + filter.Value + "|"));
            databaseReport.Name = model.Name;
            databaseReport.ChartType = model.ChartType;
            databaseReport.ColumnNames = columnListString;
            databaseReport.EndDateTime = model.EndDateTime;
            databaseReport.StartDateTime = model.StartDateTime;
            databaseReport.GraphType = model.GraphType;
            databaseReport.DynamicReportFolderId = model.DynamicReportFolderId;
            databaseReport.TableName = model.InitialTable;
            databaseReport.TimeFrameEnum = model.TimeFrameEnum;
            databaseReport.FiltersList = filtersListString;

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

        public IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReportDbModel>> GetAllFolders()
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
        public DTResult<DynamicReportDbModel> GetFilteredReports(DTParameters param)
        {
            var filtered = _context.Filter<DynamicReportDbModel>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount);

            var finalResult = new DTResult<DynamicReportDbModel>
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
        public void CreateReport(DynamicReport reportModel)
        {
            var report = ParseForDbModel(new DynamicReportDbModel(), reportModel);
            _context.DynamicReports.Add(report);
            _context.SaveChanges();
        }

        /// <summary>
        /// Clone from Existing
        /// </summary>
        /// <param name="id"></param>
        public DynamicReport CloneReport(Guid id)
        {
            return DeepClone(ParseReport(id));
        }

        /// <summary>
        /// Parse Report
        /// Get one from db and parse to working version
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DynamicReport ParseReport(Guid id)
        {
            var model = _context.DynamicReports.First(x => x.Id == id);

            var response = new DynamicReport()
            {
                Id = model.Id,
                Name = model.Name,
                StartDateTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
                ChartType = model.ChartType,
                GraphType = model.GraphType,
                InitialTable = model.TableName,
                TimeFrameEnum = model.TimeFrameEnum,
                IsDeleted = model.IsDeleted,
                Author = model.Author,
                Changed = model.Changed,
                ModifiedBy = model.ModifiedBy,
                Created = model.Created,
                ColumnList = new List<DynamicReportColumnDataModel>(),
                Filters = new List<DynamicReportFilter>(),
                DynamicReportFolderId = model.DynamicReportFolderId
            };

            //TODO: Check parser for update
            foreach (var column in model.ColumnNames.Split("|"))
            {
                if (column == "") continue;
                response.ColumnList.Add(new DynamicReportColumnDataModel()
                {
                    DataColumn = column.Substring(column.IndexOf("(", StringComparison.Ordinal) + 1).Replace(")", ""),
                    Prefix = column.Substring(0, column.IndexOf("(", StringComparison.Ordinal))
                });
            }

            foreach (var filter in model.FiltersList.Split("|"))
            {
                if (filter == "") continue;
                var filterContent = filter.Split(":");
                response.Filters.Add(new DynamicReportFilter()
                {
                    FilterType = filterContent[0],
                    ColumnName = filterContent[1],
                    Operation = filterContent[2],
                    Value = filterContent[3]
                });
            }
            return response;
        }

        /// <summary>
        /// Edit existing report
        /// </summary>
        /// <param name="model"></param>
        public void EditReport(DynamicReport model)
        {
            var report = _context.DynamicReports.First(x => x.Id == model.Id);
            _context.Update(ParseForDbModel(report, model));
            _context.SaveChanges();
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
                    command.CommandText = "SELECT Distinct TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA LIKE 'indrivo'";
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
                    command.CommandText = "SELECT TABLE_SCHEMA FROM information_schema.TABLES WHERE TABLE_NAME LIKE '" + tableName + "' AND TABLE_SCHEMA = 'indrivo'";
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
            var query = $"SELECT column_name FROM information_schema.columns WHERE table_schema = 'indrivo' AND table_name = '{tableName}'";
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
                            "Where CU.COLUMN_NAME = '" + columnName + "' AND PK.TABLE_SCHEMA = 'indrivo'";

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

        /// <summary>
        /// Build query and
        /// Get data from the database
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="filtersList"></param>
        /// <returns></returns>
        public List<DynamicReportQueryResultViewModel> GetContent(string tableName,
            IEnumerable<DynamicReportColumnDataModel> columnNames, DateTime startDateTime, DateTime endDateTime, List<DynamicReportFilter> filtersList)
        {
            if (_context == null) throw new ArgumentNullException(nameof(_context));
            if (columnNames == null) return new List<DynamicReportQueryResultViewModel>();

            var columnList = "";
            var tableJoinerList = "\"" + GetTableSchema(tableName) + "\".\"" + tableName + "\"";

            var reportDynamicDataModels = columnNames.ToList();

            foreach (var column in reportDynamicDataModels)
            {
                var primaryTableName = GetPrimaryTableName(column.DataColumn);
                if (primaryTableName == "")
                {
                    if (column.Prefix != string.Empty)
                    {
                        columnList += ", " + column.Prefix + "\"" + tableName + "\".\"" + column.DataColumn + "\"";
                    }
                    else
                    {
                        columnList += ", \"" + tableName + "\".\"" + column.DataColumn + "\"";
                    }
                }
                else
                {
                    var fkTable = primaryTableName;
                    var tableSchema = GetTableSchema(fkTable);

                    var joinType = " INNER JOIN ";

                    //if (GetColumnType(tableName, column.DataColumn).ToString() == "Nullable`1") joinType = " LEFT OUTER JOIN ";

                    if (fkTable == "Users" && tableSchema == "Identity")
                    {
                        if (column.Prefix != string.Empty)
                        {
                            columnList += ", " + column.Prefix + "\"" + fkTable + "\".UserName\" AS " + fkTable + "Name ";
                        }
                        else
                        {
                            columnList += ", " + fkTable + ".\"UserName\" AS " + fkTable + "\"Name\" ";
                        }
                    }
                    else
                    {
                        if (column.Prefix != null)
                        {
                            columnList += ", "/* + fkTable + ".Name "+ ", "*/ + column.Prefix + "\"" + fkTable + "\".\"Name\" AS " + fkTable + "Name ";
                        }
                        else
                        {
                            columnList += ", \"" + fkTable + "\".\"Name\" AS " + fkTable + "Name ";
                        }
                    }

                    tableJoinerList += joinType + "\"" + tableSchema + "\".\"" + fkTable +
                                       "\" ON \"" + tableName + "\".\"" + column.DataColumn + "\" = \"" + fkTable + "\".\"Id\"";
                }
            }

            var command = "SELECT " + columnList.Substring(1, columnList.Length - 1) + " FROM " + tableJoinerList + " WHERE \"" + tableName + "\".\"Changed\" BETWEEN '" +
                          startDateTime.Date + "' AND '" + endDateTime.Date + "'";

            if (filtersList.Any())
            {
                var filterCommand = "";

                foreach (var filter in filtersList)
                {
                    var tablePrimaryName = GetPrimaryTableName(filter.ColumnName);
                    if (tablePrimaryName == string.Empty)
                    {
                        var tableSchema = GetTableSchema(tableName);
                        if (filter.Operation == "MIN" || filter.Operation == "MAX" || filter.Operation == "AVG")
                        {
                            filterCommand += " " + filter.FilterType + " " + filter.ColumnName + " = (SELECT " + filter.Operation + "\"" + tableName + "\".\"" + filter.ColumnName
                                             + "\" FROM \"" + tableSchema + "\".\"" + tableName + "\")";
                        }
                        else
                        {
                            filterCommand += " " + filter.FilterType + " " + tableName + "\".\"" + filter.ColumnName
                                             + "\" " + filter.Operation;
                        }

                        if (filter.Value != null)
                        {
                            filterCommand += " '" + filter.Value + "'";
                        }
                    }
                    else
                    {
                        if (tablePrimaryName != "" && (filter.FilterType == "GROUP BY" || filter.FilterType == "ORDER BY"))
                        {
                            filterCommand += " " + filter.FilterType + " " + tablePrimaryName + ".\"Name\"";
                        }
                        else
                        {
                            filterCommand += " " + filter.FilterType + " \"" + tableName + "\".\"" + filter.ColumnName
                                             + "\" " + filter.Operation;
                        }

                        if (filter.Value != null)
                        {
                            filterCommand += " '" + filter.Value + "'";
                        }
                    }

                }

                command += "" + filterCommand;
            }

            var resultContent = new List<DynamicReportQueryResultViewModel>();


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


                                if (array.Count < columnList.Split(",").Length - 1) array.Add("Total");

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

        /// <summary>
        /// Get chart data for specific timeFrame of days
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="filters"></param>
        /// <param name="chartDto"></param>
        /// <param name="timeFrame"></param>
        /// <returns></returns>
        public List<decimal> GetChartDataForTimeFrame(string tableName, List<DynamicReportColumnDataModel> columnList,
            DateTime startDateTime, DateTime endDateTime, List<DynamicReportFilter> filters, DynamicReportChartDto chartDto, int timeFrame)
        {
            if (chartDto == null) return null;

            var queryResultChart = new List<decimal>();

            for (var date = startDateTime; date.Date <= endDateTime.Date; date = date.AddDays(timeFrame))
            {
                switch (chartDto.ChartType)
                {
                    case ChartType.Total:
                        decimal.TryParse(
                            GetContent(tableName, columnList, date, date.AddDays(timeFrame), filters).First().Columns.First().Value,
                            out var value);
                        queryResultChart.Add(value);
                        break;
                    case ChartType.Count:
                        queryResultChart.Add(GetContent(tableName, columnList, date, date.AddDays(timeFrame), filters).Count);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return queryResultChart;
        }

        #endregion
    }
}