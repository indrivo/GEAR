using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.UserPreferences.Impl.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class UserPreferencesDbContextFactory : IDesignTimeDbContextFactory<UserPreferencesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public UserPreferencesDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<UserPreferencesDbContext, UserPreferencesDbContext>.CreateFactoryDbContext();
        }
    }
}