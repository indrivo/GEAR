using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ST.Audit.Extensions;
using ST.Audit.Interfaces;
using ST.Audit.Models;

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
            this.EnableIdentityTracking<TrackerIdentityDbContext<TUser, TRole, TKey>, TUser, TRole, TKey>();
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
        /// On update object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry Update(object entity)
        {
            Entry(entity).State = EntityState.Modified;
            return base.Update(entity);
        }

        /// <summary>
        /// On update
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            return base.Update(entity);
        }
    }
}