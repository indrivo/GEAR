using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.Files.Abstraction;
using GR.Files.Abstraction.Models;

namespace GR.Files.Data
{
    public class FileDbContext : TrackerDbContext, IFileContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "File";

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
        public DbSet<FileStorage> Files { get; set; }


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
        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IBaseModel
        {
            return Set<TEntity>();
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public Task InvokeSeedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
