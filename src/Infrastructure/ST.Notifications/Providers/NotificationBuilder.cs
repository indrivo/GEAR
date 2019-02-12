using System.Collections.Generic;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Notifications.Abstraction;

namespace ST.Notifications.Providers
{
    public class NotificationBuilder : INotificationBuilder
    {
        public IList<EntityViewModel> CreateNotifications()
        {
            var result = new List<EntityViewModel>();
            return result;
        }
    }
}