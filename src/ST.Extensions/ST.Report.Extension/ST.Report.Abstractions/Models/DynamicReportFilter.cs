using ST.Report.Abstractions.Models.Enums;

namespace ST.Report.Abstractions.Models
{
    /// <summary>
    /// Dynamic Report Filters
    /// </summary>
    public sealed class DynamicReportFilter
    {
        public string FieldName { get; set; }
        public FilterType FilterType { get; set; }
        public string Value { get; set; }
    }
}