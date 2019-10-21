using GR.Report.Abstractions.Models.Enums;

namespace GR.Report.Abstractions.Models.Dto
{
    public class DynamicReportChartFieldDto
    {
        public int FieldIndex { get; set; }

        public string FieldName { get; set; }

        public ChartFieldType ChartFieldType { get; set; }

    }
}
