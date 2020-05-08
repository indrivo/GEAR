using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.AccountActivity.Impl.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class UserActivityDbContextFactory : IDesignTimeDbContextFactory<UserActivityDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public UserActivityDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<UserActivityDbContext, UserActivityDbContext>.CreateFactoryDbContext();
        }
    }
}