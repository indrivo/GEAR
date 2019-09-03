using Microsoft.EntityFrameworkCore;
using ST.Audit.Abstractions.Models;

namespace ST.Audit.Abstractions
{
    public interface ITrackerDbContext
    {
        /// <summary>
        /// Track audit entity
        /// </summary>
        DbSet<TrackAudit> TrackAudits { get; set; }

        /// <summary>
        /// Track audit details
        /// </summary>
        DbSet<TrackAuditDetails> TrackAuditDetails { get; set; }
    }
}