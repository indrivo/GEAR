using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Abstractions;
using ST.Audit.Abstractions.Helpers;
using ST.Audit.Abstractions.Models;

namespace ST.Audit.Contexts
{
    public class TrackerIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, ITrackerDbContext
        where TUser : IdentityUser<TKey> where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public TrackerIdentityDbContext(DbContextOptions options) : base(options)
        {
            //Enable tracking for identity db context
            //this.EnableIdentityTracking<TrackerIdentityDbContext<TUser, TRole, TKey>, TUser, TRole, TKey>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Audit tracking entity
        /// </summary>
        public DbSet<TrackAudit> TrackAudits { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Audit track details
        /// </summary>
        public DbSet<TrackAuditDetails> TrackAuditDetails { get; set; }


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