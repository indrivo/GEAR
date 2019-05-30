using System.Collections.Generic;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;

namespace ST.Notifications.Abstraction
{
    public interface INotificationBuilder
    {
        IList<EntityViewModel> CreateNotifications();
    }
}
