using System;
using System.Collections.Generic;
using ST.Core.Helpers;
using ST.Entities.ViewModels.DynamicEntities;

namespace ST.Notifications.Abstraction
{
    public interface INotificationProvider
    {
        ResultModel<bool> RestoreFromTrash(IEnumerable<Guid> notifications);

        ResultModel<int> GetUnreadNotifications(string userId);

        ResultModel<EntityViewModel> GetUserFolders(string author, bool withCount = false);

        ResultModel<List<Guid>> Create(EntityViewModel model);

        ResultModel<bool> Send(Guid notificationId, List<Guid> recipients);

        ResultModel<EntityViewModel> ListNotificationsByFolder(EntityViewModel model);

        ResultModel<EntityViewModel> ListSentNotifications(EntityViewModel model);

        ResultModel<EntityViewModel> ListReceivedNotifications(EntityViewModel model);

        ResultModel<bool> MarkAsRead(List<Guid> notifications);

        ResultModel<bool> ChangeFolder(EntityViewModel model);

        ResultModel<EntityViewModel> GetNotificationById(Guid notificationId);
    }
}
