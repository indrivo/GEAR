using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<ApplicationDbContext, ApplicationDbContext>.CreateFactoryDbContext();
        }
    }
}