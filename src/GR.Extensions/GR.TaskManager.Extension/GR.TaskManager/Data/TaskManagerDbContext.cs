using System;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.TaskManager.Abstractions;
using GR.TaskManager.Abstractions.Models;

namespace GR.TaskManager.Data
{
    public class TaskManagerDbContext : TrackerDbContext, ITaskManagerContext
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
        }

        /// <inheritdoc />
        /// <summary>
        /// Tasks
        /// </summary>
        public virtual DbSet<Task> Tasks { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Task items
        /// </summary>
        public virtual DbSet<TaskItem> TaskItems { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Task Assigned Users
        /// </summary>
        public virtual DbSet<TaskAssignedUser> TaskAssignedUsers { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<Task>()
                .HasMany(x => x.TaskItems);
            builder.Entity<Task>()
                .HasIndex(p => p.UserId);
            builder.Entity<Task>()
                .HasIndex(p => new { p.UserId, p.IsDeleted });
            builder.Entity<Task>()
                .HasIndex(p => new { p.Id, p.IsDeleted });
            builder.Entity<Task>()
                .HasIndex(p => new { p.EndDate });
            builder.Entity<TaskItem>()
                .HasKey(x => new { x.Id });
            builder.Entity<TaskItem>()
                .HasOne(p => p.Task).WithMany(x => x.TaskItems)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TaskAssignedUser>().HasKey(x => new { x.TaskId, x.UserId });
            builder.Entity<Task>()
                .HasMany(x => x.AssignedUsers)
                .WithOne(x => x.Task)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task InvokeSeedAsync(IServiceProvider services)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
