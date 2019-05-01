using ST.Audit.Models;

namespace ST.WebHost.ViewModels.AuditViewModels
{
	public class TrackAuditDetailVersions : TrackAuditDetails
	{
		public int Version { get; set; }
	}
}