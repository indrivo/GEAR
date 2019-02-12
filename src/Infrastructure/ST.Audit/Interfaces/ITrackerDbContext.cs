using Microsoft.EntityFrameworkCore;
using ST.Audit.Models;
using System;
using System.Collections.Generic;

namespace ST.Audit.Interfaces
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