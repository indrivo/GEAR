using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.EntityFrameworkCore;

namespace GR.Notifications.Abstractions
{
    public interface INotificationsContext
    {
        DbSet<Notification> Notifications { get; set; }
    }
}