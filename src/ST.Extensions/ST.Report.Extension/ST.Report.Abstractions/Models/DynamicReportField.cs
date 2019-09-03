using ST.Report.Abstractions.Models.Enums;

namespace ST.Report.Abstractions.Models
{
    /// <summary>
    /// Dynamic Report Field
    /// </summary>
    public sealed class DynamicReportField
    {
        public string FieldName { get; set; }
        public string FieldAlias { get; set; }
        public AggregateType AggregateType { get; set; }
    }
}
