using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Data
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<GearIdentityDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GearIdentityDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<GearIdentityDbContext, GearIdentityDbContext>.CreateFactoryDbContext();
        }
    }
}