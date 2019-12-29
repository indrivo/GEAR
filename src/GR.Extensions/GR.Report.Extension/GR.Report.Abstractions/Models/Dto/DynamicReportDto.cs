using System.Collections.Generic;

namespace GR.Report.Abstractions.Models.Dto
{
    public class DynamicReportDto
    {
        public IEnumerable<string> Tables { get; set; } = new List<string>();
        public IEnumerable<DynamicReportRelationDto> Relations { get; set; } = new List<DynamicReportRelationDto>();
        public IEnumerable<DynamicReportFieldDto> FieldsList { get; set; } = new List<DynamicReportFieldDto>();
        public IEnumerable<DynamicReportFilterDto> FiltersList { get; set; } = new List<DynamicReportFilterDto>();
        public IEnumerable<DynamicReportChartDto> DynamicReportCharts { get; set; } = new List<DynamicReportChartDto>();
    }
}
