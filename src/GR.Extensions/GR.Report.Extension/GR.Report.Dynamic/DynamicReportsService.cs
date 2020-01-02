using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using GR.Core;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions;
using GR.Report.Abstractions;
using GR.Report.Abstractions.Extensions;
using GR.Report.Abstractions.Helpers;
using GR.Report.Abstractions.Models;
using GR.Report.Abstractions.Models.Dto;
using GR.Report.Abstractions.Models.Enums;
using GR.Report.Abstractions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Extensions;

namespace GR.Report.Dynamic
{
    public class DynamicReportsService<TContext> : IDynamicReportsService where TContext : DbContext, IReportContext
    {
        private readonly TContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserManager<GearUser> _userManager;
        private readonly ResultModel<GearUser> _user;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        public DynamicReportsService(TContext context, IConfiguration configuration, IUserManager<GearUser> userManager, IOrganizationService<Tenant> organizationService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _organizationService = organizationService;
            _user = _userManager.GetCurrentUserAsync().GetAwaiter().GetResult();
            if (_context == null) throw new ArgumentNullException(nameof(_context));
        }

        #region Report Folders

        public ResultModel<bool> CreateFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName)) return ResultMessagesEnum.FolderNameNullOrEmpty.ToErrorModel<bool>();

            try
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
                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    KeyEntity = folder.Id
                };
            }
            catch
            {
                return ResultMessagesEnum.FolderNotSaved.ToErrorModel<bool>();
            }
        }


        public ResultModel<DynamicReportFolderViewModel> GetFolder(Guid folderId)
        {
            if (folderId == Guid.Empty) return ResultMessagesEnum.FolderNotFound.ToErrorModel<DynamicReportFolderViewModel>();

            var reportFolder = _context.DynamicReportsFolders.First(x => x.Id == folderId);

            if (reportFolder == null)
            {
                return ResultMessagesEnum.FolderNotFound.ToErrorModel<DynamicReportFolderViewModel>();
            }

            return new ResultModel<DynamicReportFolderViewModel>
            {
                IsSuccess = true,
                KeyEntity = reportFolder.Id,
                Result = new DynamicReportFolderViewModel(reportFolder.Id, reportFolder.Name)
            };
        }


        public ResultModel<bool> EditFolder(DynamicReportFolderViewModel folderModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderModel.Name)) return ResultMessagesEnum.FolderNameNullOrEmpty.ToErrorModel<bool>();

                var entity = _context.DynamicReportsFolders.First(x => x.Id == folderModel.Id);

                if (entity == null)
                {
                    return ResultMessagesEnum.FolderNotFound.ToErrorModel<bool>();
                }

                entity.Name = folderModel.Name;
                entity.ModifiedBy = _user.Result.UserName;
                entity.Changed = DateTime.Now;
                _context.DynamicReportsFolders.Update(entity);
                _context.SaveChanges();
                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    KeyEntity = entity.Id
                };
            }
            catch
            {
                return ResultMessagesEnum.FolderNotSaved.ToErrorModel<bool>();
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Delete folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteFolderAsync(Guid? folderId)
        {
            if (!folderId.HasValue) return ResultMessagesEnum.FolderNotFound.ToErrorModel();
            var reportFolder = await _context.DynamicReportsFolders
                .Include(s => s.Reports).FirstOrDefaultAsync(x => x.Id == folderId);

            if (reportFolder == null)
            {
                return ResultMessagesEnum.FolderNotFound.ToErrorModel();
            }

            if (reportFolder.Reports.Any())
            {
                return ResultMessagesEnum.FolderNotEmpty.ToErrorModel();
            }

            _context.DynamicReportsFolders.Remove(reportFolder);
            var dbResult = await _context.SaveAsync();

            if (dbResult.IsSuccess.Negate()) return ResultMessagesEnum.FolderNotDeleted.ToErrorModel();
            dbResult.KeyEntity = folderId;
            return dbResult;
        }


        public IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReport>> GetAllFolders() => _context.DynamicReportsFolders.Include(x => x.Reports);
        #endregion

        #region Reports

        public DTResult<DynamicReportViewModel> GetFilteredReports(DTParameters param)
        {
            var filtered = _context.Filter<DynamicReport>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount).Select(x =>
            {
                x.DynamicReportFolder = _context.DynamicReportsFolders.FirstOrDefault(y => y.Id == x.DynamicReportFolderId);
                return x;
            }).ToList();

            var finalResult = new DTResult<DynamicReportViewModel>
            {
                Draw = param.Draw,
                Data = filtered.Select(x => new DynamicReportViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ReportDataModel = x.ReportDataModel,
                    DynamicReportFolder = new DynamicReportFolderViewModel(x.DynamicReportFolder.Id, x.DynamicReportFolder.Name),
                    Author = x.Author,
                    ModifiedBy = x.ModifiedBy,
                    Created = x.Created,
                    Changed = x.Changed,
                    IsDeleted = x.IsDeleted
                }).ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return finalResult;
        }

        public ResultModel<bool> CreateReport(DynamicReportViewModel reportModel)
        {
            try
            {
                if (reportModel.DynamicReportFolder == null || reportModel.DynamicReportFolder.Id == Guid.Empty) return ResultMessagesEnum.FolderNotFound.ToErrorModel<bool>();

                var reportFolder = _context.DynamicReportsFolders.First(x => x.Id == reportModel.DynamicReportFolder.Id);

                if (reportFolder == null)
                {
                    return ResultMessagesEnum.FolderNotFound.ToErrorModel<bool>();
                }

                var reportDb = new DynamicReport()
                {
                    Id = Guid.NewGuid(),
                    Name = reportModel.Name,
                    ReportDataModel = reportModel.ReportDataModel,
                    DynamicReportFolderId = reportModel.DynamicReportFolder.Id,
                    Author = _user.Result.UserName,
                    Created = DateTime.Now
                };

                _context.DynamicReports.Add(reportDb);
                _context.SaveChanges();
                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    KeyEntity = reportModel.Id
                };
            }
            catch
            {
                return ResultMessagesEnum.ReportNotSaved.ToErrorModel<bool>();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Create Report
        /// </summary>
        /// <param name="reportModel"></param>
        public ResultModel<bool> EditReport(DynamicReportViewModel reportModel)
        {
            try
            {
                if (reportModel.DynamicReportFolder == null || reportModel.DynamicReportFolder.Id == Guid.Empty) return ResultMessagesEnum.FolderNotFound.ToErrorModel<bool>();

                var reportFolder = _context.DynamicReportsFolders.First(x => x.Id == reportModel.DynamicReportFolder.Id);

                if (reportFolder == null)
                {
                    return ResultMessagesEnum.FolderNotFound.ToErrorModel<bool>();
                }

                var report = _context.DynamicReports.First(x => x.Id == reportModel.Id);

                if (report == null)
                {
                    return ResultMessagesEnum.ReportNotFound.ToErrorModel<bool>();
                }

                report.Name = reportModel.Name;
                report.ReportDataModel = reportModel.ReportDataModel;
                report.IsDeleted = reportModel.IsDeleted;
                report.ModifiedBy = _user.Result.UserName;
                report.Changed = DateTime.Now;
                report.DynamicReportFolderId = reportModel.DynamicReportFolder.Id;
                _context.Update(report);
                _context.SaveChanges();
                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    KeyEntity = report.Id
                };
            }
            catch
            {
                return ResultMessagesEnum.ReportNotSaved.ToErrorModel<bool>();
            }
        }

        public ResultModel<DynamicReportViewModel> GetReport(Guid reportId)
        {
            var report = _context.DynamicReports.Include(s => s.DynamicReportFolder).FirstOrDefault(x => x.Id == reportId);

            if (report == null)
            {
                return ResultMessagesEnum.ReportNotFound.ToErrorModel<DynamicReportViewModel>();
            }

            return new ResultModel<DynamicReportViewModel>
            {
                IsSuccess = true,
                Result = new DynamicReportViewModel
                {
                    Id = report.Id,
                    DynamicReportFolder = new DynamicReportFolderViewModel(report.DynamicReportFolder.Id, report.DynamicReportFolder.Name),
                    Name = report.Name,
                    ReportDataModel = report.ReportDataModel
                }
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete report
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteReportAsync(Guid? reportId)
        {
            if (!reportId.HasValue) return ResultMessagesEnum.ReportNotFound.ToErrorModel();
            var report = await _context.DynamicReports.FirstOrDefaultAsync(x => x.Id == reportId);

            if (report == null)
            {
                return ResultMessagesEnum.ReportNotFound.ToErrorModel();
            }

            _context.DynamicReports.Remove(report);

            var dbResult = await _context.SaveAsync();
            return !dbResult.IsSuccess ? ResultMessagesEnum.ReportNotDeleted.ToErrorModel() : dbResult;
        }

        #endregion

        #region Database Data Gathering Region

        /// <summary>
        /// Get Current ConnectionString
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            var (_, connection) = _configuration.GetConnectionStringInfo();
            return connection;
        }

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
                            while (reader.Read())
                                yield return SqlDataReaderToExpando(reader);

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
                schemas = tenants.Any(s => s.Id == userTenant.Value && s.Name == GearSettings.DEFAULT_ENTITY_SCHEMA)
                    ? tenants.Select(s => s.Name).ToList()
                    : tenants.Where(s => s.Id == userTenant).Select(s => s.Name).ToList();
            return schemas;
        }



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
                            while (reader.Read())
                                yield return (reader.GetString(0));

                        reader.Close();
                    }
                }

                connection.Close();
            }
        }


        public ResultModel<IEnumerable<dynamic>> GetReportContent(DynamicReportDto reportModel)
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
                        var queryBuilder = new StringBuilder();
                        queryBuilder.Append("SELECT ");
                        if (!reportModel.FieldsList.Any())
                        {
                            queryBuilder.Append(" * ");
                        }
                        else
                        {
                            foreach (var field in reportModel.FieldsList)
                            {
                                queryBuilder.Append(field.AggregateType != AggregateType.None
                                    ? $" {field.AggregateType.GetDescription()}({field.FieldName}) {(string.IsNullOrWhiteSpace(field.FieldAlias) ? "" : $" AS \"{field.FieldAlias}\"")}"
                                    : $" {field.FieldName} {(string.IsNullOrWhiteSpace(field.FieldAlias) ? "" : $" AS \"{field.FieldAlias}\"")}");

                                if (!field.Equals(reportModel.FieldsList.Last()))
                                {
                                    queryBuilder.Append($",");
                                }
                            }
                        }

                        queryBuilder.Append(" FROM ");

                        reportModel.Tables = reportModel.Tables.OrderByDescending(s => reportModel.Relations.Any(x => x.PrimaryKeyTable == s)).ToList();

                        foreach (var table in reportModel.Tables)
                        {
                            var tableName = string.Empty;
                            var schema = string.Empty;
                            var tableParts = table.Split('.');
                            if (tableParts.Length > 1)
                            {
                                tableName = tableParts[1];
                                schema = tableParts[0];
                            }

                            if (table.Equals(reportModel.Tables.First()))
                            {
                                queryBuilder.Append($@" {schema}.""{tableName}"" ");
                            }
                            else
                            {
                                queryBuilder.Append($@" LEFT JOIN {schema}.""{tableName}"" ");
                                var rel = reportModel.Relations.FirstOrDefault(s => s.ForeignKeyTable == table || s.PrimaryKeyTable == table);
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

                        var filterFields = reportModel.FiltersList.Where(s => s.FilterType != FilterType.GroupBy).ToList();


                        foreach (var filter in filterFields)
                        {
                            queryBuilder.Append(filter.Equals(filterFields.First())
                                ? $@" WHERE {filter.FieldName} {filter.FilterType.GetDisplayName()} {filter.Value} "
                                : $@" AND {filter.FieldName} {filter.FilterType.GetDisplayName()} {filter.Value} ");
                        }

                        var groupByFields = reportModel.FiltersList.Where(s => s.FilterType == FilterType.GroupBy).ToList();

                        foreach (var group in groupByFields)
                        {
                            queryBuilder.Append(@group.Equals(groupByFields.First())
                                ? $@" GROUP BY {@group.FieldName} "
                                : $@", {@group.FieldName} ");
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


        public IEnumerable<SelectOption> GetChartFieldTypes(ChartType chartType)
        {
            var resultDict = Enum<ChartFieldType>.ToDictionary().ToList();
            switch (chartType)
            {
                case ChartType.Grid:
                    resultDict = resultDict.Where(s => s.Key == ChartFieldType.Normal).ToList();
                    break;
                case ChartType.PivotGrid:
                case ChartType.Line:
                    resultDict = resultDict.Where(s => new List<ChartFieldType> { ChartFieldType.Label, ChartFieldType.XAxis, ChartFieldType.YAxis }.Contains(s.Key)).ToList();
                    break;
                case ChartType.BarHorizontal:
                case ChartType.BarVertical:
                case ChartType.Pie:
                case ChartType.Doughnut:
                    resultDict = resultDict.Where(s => new List<ChartFieldType> { ChartFieldType.Label, ChartFieldType.XAxis }.Contains(s.Key)).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(chartType), chartType, null);
            }

            return resultDict.Select(s => new SelectOption { Id = (int)s.Key, Text = s.Value }).ToList();
        }

        #endregion
    }
}