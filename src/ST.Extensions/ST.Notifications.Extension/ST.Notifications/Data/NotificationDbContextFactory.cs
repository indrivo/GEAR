using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Notifications.Data
{
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
