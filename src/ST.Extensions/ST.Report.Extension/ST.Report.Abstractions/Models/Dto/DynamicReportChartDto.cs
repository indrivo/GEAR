using System.Collections.Generic;
using ST.Report.Abstractions.Models.Enums;

namespace ST.Report.Abstractions.Models.Dto
{
    public sealed class DynamicReportChartDto
    {
        public string ChartTitle { get; set; }

        public ChartType ChartType { get; set; }

        public IEnumerable<DynamicReportChartFieldDto> DynamicReportChartFields { get; set; } = new List<DynamicReportChartFieldDto>();
    }
}
