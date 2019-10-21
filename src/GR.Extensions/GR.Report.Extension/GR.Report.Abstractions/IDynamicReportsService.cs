using Microsoft.EntityFrameworkCore.Query;
using GR.Core;
using GR.Core.Helpers;
using GR.Report.Abstractions.Models;
using GR.Report.Abstractions.Models.Dto;
using GR.Report.Abstractions.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Report.Abstractions.Models.Enums;

namespace GR.Report.Abstractions
{
    public interface IDynamicReportsService
    {
        /// <summary>
        /// Get reports
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        DTResult<DynamicReportViewModel> GetFilteredReports(DTParameters param);

        /// <summary>
        /// Create Folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        ResultModel<bool> CreateFolder(string folderName);

        /// <summary>
        /// Get folder by id
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        ResultModel<DynamicReportFolderViewModel> GetFolder(Guid folderId);

        /// <summary>
        /// Edit report folder
        /// </summary>
        /// <param name="folderModel"></param>
        /// <returns></returns>
        ResultModel<bool> EditFolder(DynamicReportFolderViewModel folderModel);

        /// <summary>
        /// Delete folder by id
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteFolderAsync(Guid? folderId);

        /// <summary>
        /// Get report folders
        /// </summary>
        IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReport>> GetAllFolders();

        /// <summary>
        /// Create Report
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        ResultModel<bool> CreateReport(DynamicReportViewModel reportModel);

        /// <summary>
        /// Edit Report
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        ResultModel<bool> EditReport(DynamicReportViewModel reportModel);

        /// <summary>
        /// Get report by id
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        ResultModel<DynamicReportViewModel> GetReport(Guid reportId);

        /// <summary>
        /// Delete report
        /// </summary>
        /// <param name="reportId"></param>
        Task<ResultModel> DeleteReportAsync(Guid? reportId);

        /// <summary>
        /// Get all table names from DB
        /// </summary>
        /// <returns></returns>
        IEnumerable<dynamic> GetTableNames();

        /// <summary>
        /// Get a list of user schemas
        /// </summary>
        IEnumerable<string> GetUserSchemas();

        /// <summary>
        /// Get a list of column names from a specific table
        /// </summary>
        /// <param name="tableFullName"></param>
        /// <returns></returns>
        IEnumerable<string> GetTableColumns(string tableFullName);

        /// <summary>
        /// Get the report execution query dynamic result by report model
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        ResultModel<IEnumerable<dynamic>> GetReportContent(DynamicReportDto reportModel);

        /// <summary>
        /// Get a list of chart field types by chart type
        /// </summary>
        /// <param name="chartType"></param>
        /// <returns></returns>
        IEnumerable<SelectOption> GetChartFieldTypes(ChartType chartType);
    }
}
