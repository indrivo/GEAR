using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.PageRender.Data
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
            return DbContextFactory<DynamicPagesDbContext, DynamicPagesDbContext>.CreateFactoryDbContext();
        }
    }
}
