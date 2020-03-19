using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Data
{
    public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IdentityDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<IdentityDbContext, IdentityDbContext>.CreateFactoryDbContext();
        }
    }
}