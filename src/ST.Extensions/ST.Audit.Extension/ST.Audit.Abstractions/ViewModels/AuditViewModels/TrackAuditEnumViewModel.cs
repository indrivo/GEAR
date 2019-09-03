using ST.Audit.Abstractions.Models;

namespace ST.Audit.Abstractions.ViewModels.AuditViewModels
{
	public class TrackAuditEnumViewModel
	{
		public string ActionUrl { get; set; }

		public TrackAudit Track { get; set; }
	}
}
