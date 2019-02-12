using ST.Audit.Models;

namespace ST.CORE.Models.AuditViewModels
{
	public class TrackAuditsListViewModel : TrackAudit
	{
		public string CreatedString { get; set; }
		public string ChangedString { get; set; }
		public string EventType { get; set; }
		public string EntityName { get; set; }
	}
}
