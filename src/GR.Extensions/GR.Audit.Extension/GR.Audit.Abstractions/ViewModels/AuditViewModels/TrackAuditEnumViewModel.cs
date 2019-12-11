using GR.Audit.Abstractions.Models;

namespace GR.Audit.Abstractions.ViewModels.AuditViewModels
{
	public class TrackAuditEnumViewModel
	{
		public string ActionUrl { get; set; }

		public TrackAudit Track { get; set; }
	}
}
