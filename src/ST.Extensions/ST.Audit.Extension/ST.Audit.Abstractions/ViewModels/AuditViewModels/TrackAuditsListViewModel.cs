using ST.Audit.Abstractions.Models;

namespace ST.Audit.Abstractions.ViewModels.AuditViewModels
{
	public class TrackAuditsListViewModel : TrackAudit
	{
		public string CreatedString { get; set; }
		public string ChangedString { get; set; }
		public string EventType { get; set; }
		public string EntityName { get; set; }
        public string ModuleName { get; set; }
	}
}
