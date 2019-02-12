using ST.Audit.Models;

namespace ST.CORE.Models.AuditViewModels
{
	public class TrackAuditDetailVersions : TrackAuditDetails
	{
		public int Version { get; set; }
	}
}