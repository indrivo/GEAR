namespace ST.Report.Abstractions.Models
{
    public class DynamicReportChartDto
    {
        public GraphType GraphType { get; set; }
        public ChartType ChartType { get; set; }
        public TimeFrameEnum TimeFrameEnum { get; set; }
    }
}