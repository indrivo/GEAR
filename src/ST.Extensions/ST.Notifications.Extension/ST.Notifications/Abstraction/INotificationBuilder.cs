using System.Collections.Generic;
using ST.Entities.ViewModels.DynamicEntities;

namespace ST.Notifications.Abstraction
{
    public interface INotificationBuilder
    {
        IList<EntityViewModel> CreateNotifications();
    }
}
