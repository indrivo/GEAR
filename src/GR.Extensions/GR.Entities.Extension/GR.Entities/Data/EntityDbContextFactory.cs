using GR.Core.Helpers.DbContexts;
using GR.Entities.Security.Data;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Entities.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class EntitiesDbContextFactory : IDesignTimeDbContextFactory<EntitiesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public EntitiesDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<EntitiesDbContext, EntitySecurityDbContext>.CreateFactoryDbContext();
        }
    }
}