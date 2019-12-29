using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Notifications.Abstractions.Models.Data;

namespace GR.Notifications.Abstractions
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