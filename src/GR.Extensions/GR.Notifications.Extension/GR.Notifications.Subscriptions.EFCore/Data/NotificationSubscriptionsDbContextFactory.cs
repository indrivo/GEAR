using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Notifications.Subscriptions.EFCore.Data
{
    /// <summary>
    /// Do not remove this
    /// It is used for generate migrations
    /// </summary>
    public class NotificationSubscriptionsDbContextFactory : IDesignTimeDbContextFactory<NotificationsSubscriptionDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public NotificationsSubscriptionDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<NotificationsSubscriptionDbContext, NotificationsSubscriptionDbContext>.CreateFactoryDbContext();
        }
    }
}
