using System;
using System.Collections.Generic;
using ST.Core.Helpers;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Notifications.Abstraction;

namespace ST.Notifications.Providers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EmailNotificationProvider : INotificationProvider
    {
        public ResultModel<bool> RestoreFromTrash(IEnumerable<Guid> notifications)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<int> GetUnreadNotifications(string userId)
        {
            var returnModel = new ResultModel<int>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<EntityViewModel> GetUserFolders(string author, bool withCount = false)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<List<Guid>> Create(EntityViewModel model)
        {
            var returnModel = new ResultModel<List<Guid>>
            {
                IsSuccess = false
            };

            return returnModel;
        }

        public ResultModel<bool> Send(Guid notificationId, List<Guid> recipients)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<EntityViewModel> ListSentNotifications(EntityViewModel model)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<EntityViewModel> ListNotificationsByFolder(EntityViewModel model)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<EntityViewModel> ListReceivedNotifications(EntityViewModel model)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<bool> ChangeFolder(EntityViewModel model)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<EntityViewModel> GetNotificationById(Guid notificationId)
        {
            var returnModel = new ResultModel<EntityViewModel>
            {
                IsSuccess = false
            };
            return returnModel;
        }

        public ResultModel<bool> MarkAsRead(List<Guid> notifications)
        {
            var returnModel = new ResultModel<bool>
            {
                IsSuccess = false
            };
            return returnModel;
        }
    }
}