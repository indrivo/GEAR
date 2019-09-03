using Microsoft.EntityFrameworkCore.Query;
using ST.Core;
using ST.Core.Helpers;
using ST.Report.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ST.Report.Abstractions
{
    public interface IDynamicReportsService
    {
        DTResult<DynamicReport> GetFilteredReports(DTParameters param);
        void CreateFolder(string folderName);
        DynamicReportFolder GetFolder(Guid id);
        void EditFolder(DynamicReportFolder newFolder);
        void DeleteFolder(Guid id);
        IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReport>> GetAllFolders();
        ResultModel<bool> SaveReport(DynamicReport reportModel);
        void DeleteReport(Guid id);
        IEnumerable<dynamic> GetTableNames();
        IEnumerable<string> GetUserSchemas();
        IEnumerable<string> GetTableColumns(string tableName);
        string GetConnectionString();
        DynamicReport GetReport(Guid id);
        ResultModel<IEnumerable<dynamic>> GetReportContent(DynamicReportDataModel dto);
    }
}
