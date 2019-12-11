using GR.Report.Abstractions.Models.Enums;

namespace GR.Report.Abstractions.Models.Dto
{
    /// <summary>
    /// Dynamic Report Filters
    /// </summary>
    public sealed class DynamicReportFilterDto
    {
        public string FieldName { get; set; }
        public FilterType FilterType { get; set; }
        public string Value { get; set; }
    }
}