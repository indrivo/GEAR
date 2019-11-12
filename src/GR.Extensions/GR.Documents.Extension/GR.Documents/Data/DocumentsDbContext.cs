using GR.Audit.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using GR.Core.Abstractions;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Documents.Data
{
    public class DocumentsDbContext: TrackerDbContext, IDocumentContext
    {

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        protected DocumentsDbContext(DbContextOptions options) : base(options)
        {
            //Enable tracking
            //this.EnableTracking();
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
        /// Document versions
        /// </summary>
        public DbSet<DocumentVersion> DocumentVersions { get; set; }

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
