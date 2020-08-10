using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.IdentityDocuments.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class IdentityDocumentsDbContextFactory : IDesignTimeDbContextFactory<IdentityDocumentsDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IdentityDocumentsDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<IdentityDocumentsDbContext, IdentityDocumentsDbContext>.CreateFactoryDbContext();
        }
    }
}