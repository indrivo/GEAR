using System.Collections.Generic;
using GR.Report.Abstractions.Models.Enums;

namespace GR.Report.Abstractions.Models.Dto
{
    public sealed class DynamicReportChartDto
    {
        public string ChartTitle { get; set; }

        public ChartType ChartType { get; set; }

        public IEnumerable<DynamicReportChartFieldDto> DynamicReportChartFields { get; set; } = new List<DynamicReportChartFieldDto>();
    }
}
