using GR.Core.Abstractions;
using GR.Documents.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Documents.Abstractions
{
    public interface IDocumentContext : IDbContext
    {
        /// <summary>
        /// Documents
        /// </summary>
        DbSet<Document> Documents { get; set; }

        /// <summary>
        /// Document types
        /// </summary>
        DbSet<DocumentType> DocumentTypes { get; set; }

        /// <summary>
        /// Document versions
        /// </summary>
        DbSet<DocumentVersion> DocumentVersions { get; set; }
    }
}
