using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions;
using GR.Audit.Abstractions.Helpers;
using GR.Audit.Abstractions.Models;

namespace GR.Audit.Contexts
{
    public class TrackerDbContext : DbContext, ITrackerDbContext
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        protected TrackerDbContext(DbContextOptions options) : base(options)
        {
            //Enable tracking
            //this.EnableTracking();
        }

        /// <inheritdoc />
        /// <summary>
        /// Audit tracking entity
        /// </summary>
        public virtual DbSet<TrackAudit> TrackAudits { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Audit tracking details
        /// </summary>
        public virtual DbSet<TrackAuditDetails> TrackAuditDetails { get; set; }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            TrackerFactory.Track(this);
            return base.SaveChanges();
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            TrackerFactory.Track(this);
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}