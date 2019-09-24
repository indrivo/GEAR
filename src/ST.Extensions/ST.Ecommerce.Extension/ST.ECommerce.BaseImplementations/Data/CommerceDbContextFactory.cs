using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.ECommerce.BaseImplementations.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class CommerceDbContextDbContextFactory : IDesignTimeDbContextFactory<CommerceDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CommerceDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<CommerceDbContext, CommerceDbContext>.CreateFactoryDbContext();
        }
    }
}