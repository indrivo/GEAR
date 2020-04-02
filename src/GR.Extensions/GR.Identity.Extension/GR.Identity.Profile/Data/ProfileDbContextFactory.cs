using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Profile.Data
{
    public class ProfileDbContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ProfileDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<ProfileDbContext, ProfileDbContext>.CreateFactoryDbContext();
        }
    }
}