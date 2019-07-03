using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;
using ST.Entities.Data;

namespace ST.PageRender.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class DynamicPagesDbContextFactory : IDesignTimeDbContextFactory<DynamicPagesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DynamicPagesDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<DynamicPagesDbContext, EntitiesDbContext>.CreateFactoryDbContext();
        }
    }
}
