using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ST.Audit.Extensions;
using ST.Audit.Interfaces;
using ST.Audit.Models;

namespace ST.Audit.Contexts
{
    public class TrackerDbContext : DbContext, ITrackerDbContext
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public TrackerDbContext(DbContextOptions options) : base(options)
        {
            //Enable tracking
            this.EnableTracking();
        }

        /// <inheritdoc />
        /// <summary>
        /// Audit tracking entity
        /// </summary>
        public DbSet<TrackAudit> TrackAudits { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Audit tracking details
        /// </summary>
        public DbSet<TrackAuditDetails> TrackAuditDetails { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

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
            Entry(entity).State = EntityState.Modified;
            return base.Update(entity);
        }
    }
}