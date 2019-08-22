using Microsoft.EntityFrameworkCore.Query;
using ST.Core;
using ST.Report.Abstractions.Models;
using System;
using System.Collections.Generic;

namespace ST.Report.Abstractions
{
    public interface IDynamicReportsService
    {
        DTResult<DynamicReport> GetFilteredReports(DTParameters param);
        void CreateFolder(DynamicReportFolder name);
        DynamicReportFolder GetFolder(Guid id);
        void EditFolder(DynamicReportFolder newFolder);
        void DeleteFolder(Guid id);
        IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReport>> GetAllFolders();
        void SaveReport(DynamicReport reportModel);
        void DeleteReport(Guid id);
        IEnumerable<string> GetTableNames();
        string GetTableSchema(string tableName);
        IEnumerable<string> GetTableColumns(string tableName);
        dynamic GetColumnType(string tableName, string columnName);
        string GetConnectionString();
        string GetPrimaryTableName(string columnName);
        dynamic GetForeignKeySelectValues(string tableName, string columnName);
        DynamicReport GetReport(Guid id);
        IEnumerable<dynamic> GetReportContent(DynamicReportDataModel dto);
    }
}
