using Microsoft.EntityFrameworkCore;
using ST.Audit.Abstractions.Models;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.Files.Abstraction.Models;
using ST.Files.Box.Abstraction;
using ST.Files.Box.Abstraction.Models;

namespace ST.Files.Box.Data
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
    }
}
