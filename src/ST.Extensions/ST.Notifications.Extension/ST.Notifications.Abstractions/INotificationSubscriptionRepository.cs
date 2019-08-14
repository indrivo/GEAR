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
        Task SeedEventsAsync();
        Task<ResultModel<IEnumerable<ApplicationRole>>> GetRolesSubscribedToEventAsync(string eventName);
        Task<ResultModel<NotificationTemplate>> GetEventTemplateAsync(string eventName);
        Task<ResultModel> SubscribeRolesToEventAsync(IEnumerable<Guid> roles, string eventName, string template,
            string subject);
    }
}