using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Modules.Infrastructure.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class ModuleDbContextFactory : IDesignTimeDbContextFactory<ModuleDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ModuleDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<ModuleDbContext, ModuleDbContext>.CreateFactoryDbContext();
        }
    }
}