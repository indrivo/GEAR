using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.Files.Abstraction;
using ST.Files.Abstraction.Models;

namespace ST.Files.Data
{
   public class FileDbContext : TrackerDbContext, IFileContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "DMS";

        /// <inheritdoc />
        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        public FileDbContext(DbContextOptions<FileDbContext> options)
            : base(options)
        {
            //TODO: Do some actions on context instance
        }

        /// <summary>
        /// Files
        /// </summary>
        public virtual DbSet<File> Files { get; set; }



        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IBaseModel
        {
            return Set<TEntity>();
        }
    }
}
