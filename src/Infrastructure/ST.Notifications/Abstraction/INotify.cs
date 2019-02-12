using ST.BaseBusinessRepository;
using ST.Entities.Models.Notifications;
using ST.Identity.Data.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ST.Notifications.Abstraction
{
    public interface INotify
    {
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendNotificationAsync(IEnumerable<ApplicationRole> roles, SystemNotifications notification);
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendNotificationAsync(IEnumerable<Guid> users, SystemNotifications notification);
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notificationType"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task SendNotificationAsync(IEnumerable<Guid> users, Guid notificationType, string subject,
            string content);
        /// <summary>
        /// Send notifications to all users
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendNotificationAsync(SystemNotifications notification);
        /// <summary>
        /// Send notifications to user admins
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendNotificationToSystemAdminsAsync(SystemNotifications notification);
        /// <summary>
        /// Get notifications by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<SystemNotifications>>> GetNotificationsByUserIdAsync(Guid userId);
        /// <summary>
        /// Mark notification as read
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> MarkAsReadAsync(Guid notificationId);
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool IsUserOnline(Guid userId);
    }
}
