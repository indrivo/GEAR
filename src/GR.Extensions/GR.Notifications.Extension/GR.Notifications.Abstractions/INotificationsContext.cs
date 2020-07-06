using GR.Core.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.EntityFrameworkCore;

namespace GR.Notifications.Abstractions
{
    public interface INotificationsContext : IDbContext
    {
        /// <summary>
        /// Notifications
        /// </summary>
        DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Notification types
        /// </summary>
        DbSet<NotificationTypes> NotificationTypes { get; set; }
    }
}