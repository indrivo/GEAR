using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions.Models;
using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.Files.Box.Abstraction;
using GR.Files.Box.Abstraction.Models;

namespace GR.Files.Box.Data
{
    public class FileBoxDbContext : TrackerDbContext, IFileBoxContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "File";

        public static bool IsMigrationMode { get; set; } = false;

        /// <inheritdoc />
        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        public FileBoxDbContext(DbContextOptions<FileBoxDbContext> options)
            : base(options)
        {
            //TODO: Do some actions on context instance
        }

        /// <summary>
        /// Files
        /// </summary>
        public DbSet<FileBox> FilesBox { get; set; }


        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            if (!IsMigrationMode) return;

            builder.Ignore<TrackAudit>();
            builder.Ignore<TrackAuditDetails>();
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

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}
