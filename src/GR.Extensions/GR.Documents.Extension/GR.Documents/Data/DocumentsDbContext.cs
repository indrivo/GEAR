using GR.Audit.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;

namespace GR.Documents.Data
{
    public class DocumentsDbContext: TrackerDbContext, IDocumentContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this, is used on audit 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string Schema = "Documents";
        public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : base(options)
        {
            
        }

        /// <summary>
        /// Documents
        /// </summary>
        public DbSet<Document> Documents { get; set; }

        /// <summary>
        /// Document types
        /// </summary>
        public DbSet<DocumentType> DocumentTypes { get; set; }

        /// <summary>
        /// Document types
        /// </summary>
        public DbSet<DocumentCategory> DocumentCategories { get; set; }

        /// <summary>
        /// Document versions
        /// </summary>
        public DbSet<DocumentVersion> DocumentVersions { get; set; }


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
        public virtual Task InvokeSeedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
