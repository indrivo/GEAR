using Microsoft.EntityFrameworkCore;
using GR.Notifications.Abstractions.Models.Data;
using GR.Notifications.Data;

namespace GR.Notifications.Extensions
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Db configurations
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder RegisterNotificationDbContextBuilder(this ModelBuilder builder)
        {
            //register schema
            builder.HasDefaultSchema(NotificationDbContext.Schema);
            //register composite key
            builder.Entity<NotificationSubscription>().HasKey(p => new { p.NotificationEventId, p.RoleId });

            return builder;
        }
    }
}
