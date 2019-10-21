using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions.Models;

namespace GR.Audit.Abstractions
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