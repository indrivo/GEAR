using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.UI.Menu.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class MenuDbContextFactory : IDesignTimeDbContextFactory<MenuDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public MenuDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<MenuDbContext, MenuDbContext>.CreateFactoryDbContext();
        }
    }
}
