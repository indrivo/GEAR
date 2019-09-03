using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using ST.Core;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.MultiTenant.Abstractions;
using ST.Report.Abstractions;
using ST.Report.Abstractions.Extensions;
using ST.Report.Abstractions.Models;
using ST.Report.Abstractions.Models.Enums;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace ST.Report.Dynamic
{
    public class DynamicReportsService<TContext> : IDynamicReportsService where TContext : DbContext, IReportContext
    {
        private readonly TContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserManager<ApplicationUser> _userManager;
        private readonly ResultModel<ApplicationUser> _user;
        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        public DynamicReportsService(TContext context, IConfiguration configuration, IUserManager<ApplicationUser> userManager, IOrganizationService<Tenant> organizationService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _organizationService = organizationService;
            _user = _userManager.GetCurrentUserAsync().GetAwaiter().GetResult();
            if (_context == null) throw new ArgumentNullException(nameof(_context));

        }

        #region Maps Region

        /// <summary>
        /// Create Map
        /// </summary>
        public void CreateFolder(string folderName)
        {
            var folder = new DynamicReportFolder()
            {
                Id = new Guid(),
                Name = folderName,
                IsDeleted = false,
                Author = _user.Result.UserName,
                Created = DateTime.Now
            };
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
            newFolder.ModifiedBy = _user.Result.UserName;
            newFolder.Changed = DateTime.Now;
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
        public ResultModel<bool> SaveReport(DynamicReport reportModel)
        {
            ResultModel<bool> result = new ResultModel<bool>
            {
                IsSuccess = false
            };
            try
            {
                if (reportModel.Id == Guid.Empty)
                {
                    reportModel.Id = Guid.NewGuid();
                    reportModel.Author = _user.Result.UserName;
                    reportModel.Created = DateTime.Now;
                    _context.DynamicReports.Add(reportModel);
                    result.KeyEntity = reportModel.Id;
                }
                else
                {
                    var entity = _context.DynamicReports.First(x => x.Id == reportModel.Id);
                    entity.Name = reportModel.Name;
                    entity.ReportDataModel = reportModel.ReportDataModel;
                    entity.IsDeleted = reportModel.IsDeleted;
                    entity.ModifiedBy = _user.Result.UserName;
                    entity.Changed = DateTime.Now;
                    entity.DynamicReportFolderId = reportModel.DynamicReportFolderId;
                    _context.Update(entity);
                    result.KeyEntity = entity.Id;
                }
                _context.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Errors = new List<IErrorModel> { new ErrorModel("ServerError", ex.Message) };
            }

            return result;
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
        public IEnumerable<dynamic> GetTableNames()
        {
            var schemas = GetUserSchemas();

            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = new NpgsqlCommand($"SELECT Distinct TABLE_SCHEMA as \"id\", TABLE_NAME as \"text\" FROM information_schema.TABLES WHERE TABLE_SCHEMA = ANY(@schemas)", connection))
                {
                    command.Parameters.AddWithValue("schemas", NpgsqlDbType.Array | NpgsqlDbType.Text, schemas);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                yield return SqlDataReaderToExpando(reader);
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
        }


        public IEnumerable<string> GetUserSchemas()
        {
            var userTenant = _userManager.CurrentUserTenantId;
            var schemas = new List<string>();
            var tenants = _organizationService.GetAllTenants().Select(s => new { s.Id, Name = s.MachineName }).ToList();
            if (userTenant.HasValue)
            {
                if (tenants.Any(s => s.Id == userTenant.Value && s.Name == Settings.DEFAULT_ENTITY_SCHEMA))
                {
                    schemas = tenants.Select(s => s.Name).ToList();
                }
                else
                {
                    schemas = tenants.Where(s => s.Id == userTenant).Select(s => s.Name).ToList();
                }
            }
            return schemas;
        }


        /// <summary>
        /// Get a list of column names from a specific table
        /// </summary>
        /// <param name="tableFullName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetTableColumns(string tableFullName)
        {
            var tableName = tableFullName;
            var schema = string.Empty;

            var tableParts = tableFullName.Split('.');
            if (tableParts.Length > 1)
            {
                tableName = tableParts[1];
                schema = tableParts[0];
            }

            var query = $"SELECT column_name FROM information_schema.columns WHERE table_schema = @schema AND table_name = @tableName";

            using (var connection = new NpgsqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("schema", schema);
                    command.Parameters.AddWithValue("tableName", tableName);
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


        public ResultModel<IEnumerable<dynamic>> GetReportContent(DynamicReportDataModel dto)
        {
            var result = new ResultModel<IEnumerable<dynamic>>
            {
                IsSuccess = true
            };

            IEnumerable<dynamic> Results()
            {
                using (var connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var sqlCommand = connection.CreateCommand())
                    {
                        StringBuilder queryBuilder = new StringBuilder();
                        queryBuilder.Append("SELECT ");
                        if (!dto.FieldsList.Any())
                        {
                            queryBuilder.Append(" * ");
                        }
                        else
                        {
                            foreach (var field in dto.FieldsList)
                            {
                                if (field.AggregateType != AggregateType.None)
                                {
                                    queryBuilder.Append($" {field.AggregateType.GetDescription() }({field.FieldName}) {(string.IsNullOrEmpty(field.FieldAlias) ? "" : $" AS \"{field.FieldAlias}\"")}");
                                }
                                else
                                {
                                    queryBuilder.Append($" {field.FieldName} {(string.IsNullOrEmpty(field.FieldAlias) ? "" : $" AS \"{field.FieldAlias}\"")}");
                                }

                                if (!field.Equals(dto.FieldsList.Last()))
                                {
                                    queryBuilder.Append($",");
                                }
                            }
                        }

                        queryBuilder.Append(" FROM ");

                        dto.Tables = dto.Tables.OrderByDescending(s => dto.Relations.Any(x => x.PrimaryKeyTable == s)).ToList();

                        foreach (var table in dto.Tables)
                        {
                            var tableName = string.Empty;
                            var schema = string.Empty;
                            var tableParts = table.Split('.');
                            if (tableParts.Length > 1)
                            {
                                tableName = tableParts[1];
                                schema = tableParts[0];
                            }

                            if (table.Equals(dto.Tables.First()))
                            {
                                queryBuilder.Append($@" {schema}.""{tableName}"" ");
                            }
                            else
                            {
                                queryBuilder.Append($@" LEFT JOIN {schema}.""{tableName}"" ");
                                var rel = dto.Relations.FirstOrDefault(s => s.ForeignKeyTable == table || s.PrimaryKeyTable == table);
                                if (rel != null)
                                {
                                    var relPrimaryName = string.Empty;
                                    var relPrimarySchema = string.Empty;
                                    var relPrimaryTableParts = rel.PrimaryKeyTable.Split('.');
                                    if (relPrimaryTableParts.Length > 1)
                                    {
                                        relPrimaryName = relPrimaryTableParts[1];
                                        relPrimarySchema = relPrimaryTableParts[0];
                                    }

                                    var relForeignName = string.Empty;
                                    var relForeignSchema = string.Empty;
                                    var relForeignTableParts = rel.ForeignKeyTable.Split('.');
                                    if (relForeignTableParts.Length > 1)
                                    {
                                        relForeignName = relForeignTableParts[1];
                                        relForeignSchema = relForeignTableParts[0];
                                    }
                                    queryBuilder.Append($@" ON({relPrimarySchema}.""{relPrimaryName}"".""Id"" = {relForeignSchema}.""{relForeignName}"".""{rel.ForeignKey}"") ");
                                }
                                else
                                {
                                    queryBuilder.Append($@" ON(1=1) ");
                                }
                            }

                        }

                        var filterFields = dto.FiltersList.Where(s => s.FilterType != FilterType.GroupBy).ToList();


                        foreach (var filter in filterFields)
                        {
                            if (filter.Equals(filterFields.First()))
                            {
                                queryBuilder.Append($@" WHERE {filter.FieldName} {filter.FilterType.GetDisplayName()} {filter.Value} ");
                            }
                            else
                            {
                                queryBuilder.Append($@" AND {filter.FieldName} {filter.FilterType.GetDisplayName()} {filter.Value} ");
                            }
                        }

                        var groupByFields = dto.FiltersList.Where(s => s.FilterType == FilterType.GroupBy).ToList();

                        foreach (var group in groupByFields)
                        {
                            if (group.Equals(groupByFields.First()))
                            {
                                queryBuilder.Append($@" GROUP BY {group.FieldName} ");
                            }
                            else
                            {
                                queryBuilder.Append($@", {group.FieldName} ");
                            }
                        }

                        NpgsqlDataReader reader = null;
                        sqlCommand.CommandText = queryBuilder.ToString();
                        try
                        {
                            reader = sqlCommand.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            result.Errors = new List<IErrorModel> { new ErrorModel("ServerError", ex.Message) };
                        }

                        if (!result.IsSuccess)
                        {
                            yield return result.Errors.FirstOrDefault();
                            yield break;
                        }

                        if (reader != null)
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

            result.Result = Results();

            return result;
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