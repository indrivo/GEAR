using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Entities.Data
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
            return DbContextFactory<EntitiesDbContext, EntitiesDbContext>.CreateFactoryDbContext();
        }
    }
}
