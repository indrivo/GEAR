using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using GR.Core.Helpers;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Notifications.Abstractions.ViewModels;

namespace GR.Notifications.Abstractions
{
    public interface INotify<in TRole> where TRole : IdentityRole<string>
    {
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="notification"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task SendNotificationAsync(IEnumerable<TRole> roles, Notification notification, Guid? tenantId);

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendNotificationAsync(IEnumerable<Guid> users, Notification notification);

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
        Task SendNotificationAsync(Notification notification);

        /// <summary>
        /// Send notifications to user admins
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        Task SendNotificationToSystemAdminsAsync(Notification notification);

        /// <summary>
        /// Get notifications by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="onlyUnread"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<SystemNotifications>>> GetNotificationsByUserIdAsync(Guid userId, bool onlyUnread = true);

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

        /// <summary>
        /// Get notification by id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, object>>> GetNotificationByIdAsync(Guid? notificationId);

        /// <summary>
        /// Clear all notifications
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> ClearAllUserNotificationsAsync(Guid userId);

        /// <summary>
        /// Get notifications with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="onlyUnread"></param>
        /// <returns></returns>
        Task<ResultModel<PaginatedNotificationsViewModel>> GetUserNotificationsWithPaginationAsync(uint page = 1, uint perPage = 10, bool onlyUnread = true);

        /// <summary>
        /// Delete notification
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        Task<ResultModel> PermanentlyDeleteNotificationAsync(Guid? notificationId);
    }
}
