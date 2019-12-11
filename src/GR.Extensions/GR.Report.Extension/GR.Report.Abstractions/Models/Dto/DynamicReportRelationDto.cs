namespace GR.Report.Abstractions.Models.Dto
{
    /// <summary>
    /// Dynamic Report Relation
    /// </summary>
    public sealed class  DynamicReportRelationDto
    {
        public string PrimaryKeyTable { get; set; }
        public string ForeignKeyTable { get; set; }
        public string ForeignKey { get; set; }
    }
}
