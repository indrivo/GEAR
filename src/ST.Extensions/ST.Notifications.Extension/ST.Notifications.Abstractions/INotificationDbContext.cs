using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Notifications.Abstractions.Models.Data;

namespace ST.Notifications.Abstractions
{
    public interface INotificationDbContext : IDbContext
    {
        /// <summary>
        /// Notification events
        /// </summary>
        DbSet<NotificationEvent> NotificationEvents { get; set; }
        /// <summary>
        /// Role events subscriptions
        /// </summary>
        DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }

        /// <summary>
        /// Notification templates
        /// </summary>
        DbSet<NotificationTemplate> NotificationTemplates { get; set; }
    }
}