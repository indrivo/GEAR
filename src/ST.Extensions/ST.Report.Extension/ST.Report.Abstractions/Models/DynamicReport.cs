using System;
using System.Collections.Generic;
using ST.Core;

namespace ST.Report.Abstractions.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Model created after parsing the DbModel
    /// Easier for outside use
    /// </summary>
    [Serializable]
    public class DynamicReport : BaseModel
    {
        public string Name { get; set; }
        public List<DynamicReportFilter> Filters { get; set; }
        public string InitialTable { get; set; }
        public List<DynamicReportColumnDataModel> ColumnList { get; set; }
        public GraphType GraphType { get; set; }
        public ChartType ChartType { get; set; }
        public TimeFrameEnum TimeFrameEnum { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Guid DynamicReportFolderId { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Model to save in database
    /// </summary>
    public class DynamicReportDbModel : BaseModel
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ColumnNames { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string FiltersList { get; set; }
        public GraphType GraphType { get; set; }
        public ChartType ChartType { get; set; }
        public TimeFrameEnum TimeFrameEnum { get; set; }
        public Guid DynamicReportFolderId { get; set; }
        public DynamicReportFolder DynamicReportFolder { get; set; }
    }

}