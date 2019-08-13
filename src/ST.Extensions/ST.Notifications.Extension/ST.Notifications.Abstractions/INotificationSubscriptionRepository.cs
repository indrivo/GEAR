using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Notifications.Abstractions.Models.Data;

namespace ST.Notifications.Abstractions
{
    public interface INotificationSubscriptionRepository
    {
        IEnumerable<ApplicationRole> Roles { get; }
        IEnumerable<NotificationEvent> Events { get; }
        IEnumerable<NotificationEvent> GetSystemEvents();
        Task SeedEvents();
        Task<ResultModel<IEnumerable<ApplicationRole>>> GetRolesSubscribedToEvent(string eventName);
        Task<ResultModel<NotificationTemplate>> GetEventTemplate(string eventName);
        Task<ResultModel> SubscribeRolesToEvent(IEnumerable<Guid> roles, string eventName, string template);
    }
}