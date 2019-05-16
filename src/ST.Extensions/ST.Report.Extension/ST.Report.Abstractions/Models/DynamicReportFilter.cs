namespace ST.Report.Abstractions.Models
{
    /// <summary>
    /// Dynamic Report Filters
    /// </summary>
    public class DynamicReportFilter
    {
        public string FilterType { get; set; }
        public string ColumnName { get; set; }
        public string Operation { get; set; }
        public string Value { get; set; }
    }
}