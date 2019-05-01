using ST.Audit.Models;

namespace ST.WebHost.ViewModels.AuditViewModels
{
	public class TrackAuditEnumViewModel
	{
		public string ActionUrl { get; set; }

		public TrackAudit Track { get; set; }
	}
}
