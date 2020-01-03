using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions.Models;
using GR.Core.Abstractions;

namespace GR.Audit.Abstractions
{
    public interface ITrackerDbContext : IDbContext
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