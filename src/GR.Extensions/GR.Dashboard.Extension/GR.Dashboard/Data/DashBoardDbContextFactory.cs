using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.Dashboard.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class DashBoardDbContextDbContextFactory : IDesignTimeDbContextFactory<DashBoardDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DashBoardDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<DashBoardDbContext, DashBoardDbContext>.CreateFactoryDbContext();
        }
    }
}
