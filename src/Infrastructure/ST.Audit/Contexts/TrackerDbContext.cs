﻿using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ST.Audit.Enums;
using ST.Audit.Extensions;
using ST.Audit.Interfaces;
using ST.Audit.Models;
using ST.BaseRepository;

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

        /// <inheritdoc />
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

        /// <inheritdoc />
        /// <summary>
        /// On update
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            if (!(entity is ExtendedModel trackable)) return base.Add(entity);
            trackable.Changed = DateTime.Now;
            trackable.Version = trackable.Version + 1;
            return base.Update(entity);
        }

        /// <inheritdoc />
        /// <summary>
        /// On add
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            if (!(entity is ExtendedModel trackable)) return base.Add(entity);
            trackable.Created = DateTime.Now;
            trackable.Changed = DateTime.Now;
            //trackable.Version = 1;
            //var audit = entity.GetTrackAuditFromObject(GetType().FullName, trackable.TenantId, entity.GetType(),
            //    TrackEventType.Added);
            return base.Add(entity);
        }
    }
}