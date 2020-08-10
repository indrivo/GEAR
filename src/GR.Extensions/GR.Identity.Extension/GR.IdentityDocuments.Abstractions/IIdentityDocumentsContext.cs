using GR.Identity.Abstractions;
using GR.IdentityDocuments.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.IdentityDocuments.Abstractions
{
    public interface IIdentityDocumentsContext : IIdentityContext
    {
        /// <summary>
        /// Identity documents
        /// </summary>
        DbSet<IdentityDocument> IdentityDocuments { get; set; }
    }
}