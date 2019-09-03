using ST.Report.Abstractions.Models.Enums;
using System.Collections.Generic;

namespace ST.Report.Abstractions.Models
{
    public sealed class DynamicReportChart
    {
        public string ChartTitle { get; set; }

        public ChartType ChartType { get; set; }

        public IEnumerable<DynamicReportChartField> DynamicReportChartFields { get; set; } = new List<DynamicReportChartField>();
    }
}
