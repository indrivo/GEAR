using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Notifications.EFCore.Data
{
    /// <summary>
    /// Do not remove this
    /// It is used for generate migrations
    /// </summary>
    public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NotificationDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<NotificationDbContext, NotificationDbContext>.CreateFactoryDbContext();
        }
    }
}
