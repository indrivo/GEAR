using System;
using System.Collections.Generic;
using ST.Core.Helpers;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;
using ST.Notifications.Abstraction;

namespace ST.Notifications.Services
{
    public class Notificator
    {
        private readonly INotificationProvider _notificationProvider;

        public Notificator(INotificationProvider notificationProvider)
        {
            _notificationProvider = notificationProvider;
        }

        /// <summary>
        ///     Get unread notification
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ResultModel<int> GetUnreadNotifications(string userId)
        {
            return _notificationProvider.GetUnreadNotifications(userId);
        }

        /// <summary>
        ///     Restore from trash
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public ResultModel<bool> RestoreFromTrash(IEnumerable<Guid> notifications)
        {
            return _notificationProvider.RestoreFromTrash(notifications);
        }

        /// <summary>
        ///     Get user folders
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="withCount"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> GetUserFolders(string userId, bool withCount = false)
        {
            return _notificationProvider.GetUserFolders(userId, withCount);
        }

        /// <summary>
        ///     Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<List<Guid>> Create(EntityViewModel model)
        {
            return _notificationProvider.Create(model);
        }

        /// <summary>
        ///     Send
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="recipients"></param>
        /// <returns></returns>
        public ResultModel<bool> Send(Guid notificationId, List<Guid> recipients)
        {
            return _notificationProvider.Send(notificationId, recipients);
        }

        /// <summary>
        ///     Mark as read
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        public ResultModel<bool> MarkAsRead(List<Guid> notifications)
        {
            return _notificationProvider.MarkAsRead(notifications);
        }

        /// <summary>
        ///     Change folder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<bool> ChangeFolder(EntityViewModel model)
        {
            return _notificationProvider.ChangeFolder(model);
        }

        /// <summary>
        ///     List notification by folder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> ListNotificationsByFolder(EntityViewModel model)
        {
            return _notificationProvider.ListNotificationsByFolder(model);
        }

        /// <summary>
        ///     List send notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> ListSentNotifications(EntityViewModel model)
        {
            return _notificationProvider.ListSentNotifications(model);
        }

        public ResultModel<EntityViewModel> ListReceivedNotifications(EntityViewModel model)
        {
            return _notificationProvider.ListReceivedNotifications(model);
        }

        public ResultModel<EntityViewModel> GetNotificationById(Guid notificationId)
        {
            return _notificationProvider.GetNotificationById(notificationId);
        }
    }
}