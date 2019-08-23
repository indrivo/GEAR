using System;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.TaskManager.Abstractions;
using ST.TaskManager.Abstractions.Models;

namespace ST.TaskManager.Data
{
    public class TaskManagerDbContext: TrackerDbContext, ITaskManagerContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Task";

        /// <inheritdoc />
        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
            : base(options)
        {
            //TODO: Do some actions on context instance
        }

        /// <summary>
        /// Tasks
        /// </summary>
        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskItem> TaskItems { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<Task>().HasMany(x => x.TaskItems);
            builder.Entity<Task>()
                .HasIndex(p => p.UserId)
                .IsUnique();
            builder.Entity<Task>()
                .HasIndex(p => new {p.UserId, p.IsDeleted})
                .IsUnique();
            builder.Entity<Task>()
                .HasIndex(p => new {p.Id, p.IsDeleted})
                .IsUnique();
            builder.Entity<TaskItem>().HasKey(x => new {x.Id});
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IBaseModel
        {
            return Set<TEntity>();
        }
    }
}
