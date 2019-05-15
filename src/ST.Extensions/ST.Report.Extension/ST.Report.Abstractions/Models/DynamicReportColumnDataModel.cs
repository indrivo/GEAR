namespace ST.Report.Abstractions.Models
{
    /// <summary>
    /// Column Name model
    /// Used in specific sql queries
    /// </summary>
    public class DynamicReportColumnDataModel
    {
        public string Prefix { get; set; }
        public string DataColumn { get; set; }
    }
}