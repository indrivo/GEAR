using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions.Models.Data;

namespace GR.Notifications.Abstractions
{
    public interface INotificationSubscriptionRepository
    {
        /// <summary>
        /// Provide system roles
        /// </summary>
        IEnumerable<GearRole> Roles { get; }

        /// <summary>
        /// Provide application events
        /// </summary>
        IEnumerable<NotificationEvent> Events { get; }

        /// <summary>
        /// Provide events
        /// </summary>
        /// <returns></returns>
        IEnumerable<NotificationEvent> GetSystemEvents();

        /// <summary>
        /// Seed events in storage
        /// </summary>
        /// <returns></returns>
        Task SeedEventsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GearRole>>> GetRolesSubscribedToEventAsync(string eventName);

        /// <summary>
        /// Get template for event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Task<ResultModel<NotificationTemplate>> GetEventTemplateAsync(string eventName);

        /// <summary>
        /// Subscribe to event with a group of roles
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="eventName"></param>
        /// <param name="template"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        Task<ResultModel> SubscribeRolesToEventAsync(IEnumerable<Guid> roles, string eventName, string template,
            string subject);
    }
}