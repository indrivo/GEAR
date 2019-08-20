using ST.Report.Abstractions.Models.Enums;

namespace ST.Report.Abstractions.Models
{
    public class DynamicReportChartField
    {
        public int FieldIndex { get; set; }

        public string FieldName { get; set; }

        public ChartFieldType ChartFieldType { get; set; }

    }
}
