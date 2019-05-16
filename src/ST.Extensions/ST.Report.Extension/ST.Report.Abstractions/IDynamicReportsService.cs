using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;
using ST.Core;
using ST.Report.Abstractions.Models;

namespace ST.Report.Abstractions
{
    public interface IDynamicReportsService
    {
        DTResult<DynamicReportDbModel> GetFilteredReports(DTParameters param);
        void CreateFolder(DynamicReportFolder name);
        DynamicReportFolder GetFolder(Guid id);
        void EditFolder(DynamicReportFolder newFolder);
        void DeleteFolder(Guid id);
        IIncludableQueryable<DynamicReportFolder, IEnumerable<DynamicReportDbModel>> GetAllFolders();
        void CreateReport(DynamicReport reportModel);
        DynamicReport CloneReport(Guid id);
        DynamicReport ParseReport(Guid id);
        void EditReport(DynamicReport reportModel);
        void DeleteReport(Guid id);
        IEnumerable<string> GetTableNames();
        string GetTableSchema(string tableName);
        IEnumerable<string> GetTableColumns(string tableName);
        dynamic GetColumnType(string tableName, string columnName);
        string GetConnectionString();
        string GetPrimaryTableName(string columnName);
        dynamic GetForeignKeySelectValues(string tableName, string columnName);

        List<DynamicReportQueryResultViewModel> GetContent(string tableName,
            IEnumerable<DynamicReportColumnDataModel> columnNames, DateTime startDateTime, DateTime endDateTime,
            List<DynamicReportFilter> filtersList);

        List<decimal> GetChartDataForTimeFrame(string tableName, List<DynamicReportColumnDataModel> columnList,
            DateTime startDateTime, DateTime endDateTime, List<DynamicReportFilter> filters,
            DynamicReportChartDto chartDto, int timeFrame);
    }
}
