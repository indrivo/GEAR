namespace GR.Report.Abstractions.Models.Dto
{
    /// <summary>
    /// Column Name model
    /// Used in specific sql queries
    /// </summary>
    public class DynamicReportColumnDto
    {
        public string Prefix { get; set; }
        public string DataColumn { get; set; }
    }
}