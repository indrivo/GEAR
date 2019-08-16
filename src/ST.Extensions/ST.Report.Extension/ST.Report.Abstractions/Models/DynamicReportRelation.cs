namespace ST.Report.Abstractions.Models
{
    /// <summary>
    /// Dynamic Report Relation
    /// </summary>
    public class DynamicReportRelation
    {
        public string PrimaryKeyTable { get; set; }
        public string ForeignKeyTable { get; set; }
        public string ForeignKey { get; set; }
    }
}
