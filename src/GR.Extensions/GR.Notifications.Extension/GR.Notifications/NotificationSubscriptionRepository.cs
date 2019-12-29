using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Data;

namespace GR.Notifications
{
    public class NotificationSubscriptionRepository : INotificationSubscriptionRepository
    {
        /// <summary>
        /// Inject role manager
        /// </summary>
        private readonly RoleManager<GearRole> _roleManager;

        /// <summary>
        /// Inject notification db context
        /// </summary>
        private readonly INotificationDbContext _notificationDbContext;

        public NotificationSubscriptionRepository(RoleManager<GearRole> roleManager, INotificationDbContext notificationDbContext)
        {
            _roleManager = roleManager;
            _notificationDbContext = notificationDbContext;
        }

        /// <summary>
        /// Roles
        /// </summary>
        public IEnumerable<GearRole> Roles => _roleManager.Roles.ToList();

        /// <summary>
        /// Events
        /// </summary>
        public IEnumerable<NotificationEvent> Events => _notificationDbContext.NotificationEvents.ToList();

        /// <summary>
        /// Get template event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public async Task<ResultModel<NotificationTemplate>> GetEventTemplateAsync(string eventName)
        {
            Arg.NotNullOrEmpty(eventName, nameof(GetRolesSubscribedToEventAsync));
            var result = new ResultModel<NotificationTemplate>();
            var template =
                await _notificationDbContext.NotificationTemplates.FirstOrDefaultAsync(x =>
                    x.NotificationEventId.Equals(eventName));
            if (template == null)
            {
                result.Errors.Add(new ErrorModel(nameof(Exception), "Template not found"));
                return result;
            }

            result.IsSuccess = true;
            result.Result = template;
            return result;
        }

        /// <summary>
        /// Get subscribed roles to event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<GearRole>>> GetRolesSubscribedToEventAsync(string eventName)
        {
            Arg.NotNullOrEmpty(eventName, nameof(GetRolesSubscribedToEventAsync));
            var result = new ResultModel<IEnumerable<GearRole>>();
            var subscribed = await _notificationDbContext.NotificationSubscriptions
                .Where(x => x.NotificationEventId.Equals(eventName)).ToListAsync();
            if (!subscribed.Any()) return result;
            var roles = await _roleManager.Roles.Where(x => subscribed.Select(j => j.RoleId).Contains(x.Id.ToGuid()))
                .ToListAsync();
            result.IsSuccess = true;
            result.Result = roles;
            return result;
        }

        /// <summary>
        /// Seed events
        /// </summary>
        /// <returns></returns>
        public async Task SeedEventsAsync()
        {
            var events = GetSystemEvents();
            var dbEvents = await _notificationDbContext.NotificationEvents.ToListAsync();
            var nonSeeded = events.Where(x => !dbEvents.Select(v => v.EventId).Contains(x.EventId));
            await _notificationDbContext.NotificationEvents.AddRangeAsync(nonSeeded);
            await _notificationDbContext.PushAsync();
        }

        /// <summary>
        /// Get all system not seeded events
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NotificationEvent> GetSystemEvents()
        {
            var events = SystemEvents.Common.RegisteredEvents;
            foreach (var evGroup in events)
            {
                foreach (var ev in evGroup.Value)
                {
                    yield return new NotificationEvent
                    {
                        EventGroupName = evGroup.Key,
                        EventId = ev
                    };
                }
            }
        }

        /// <summary>
        /// Subscribe roles to event
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="eventName"></param>
        /// <param name="template"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task<ResultModel> SubscribeRolesToEventAsync(IEnumerable<Guid> roles, string eventName, string template, string subject)
        {
            var result = new ResultModel();
            if (string.IsNullOrEmpty(eventName)) return result;
            if (!SystemEvents.Common.HasEvent(eventName)) return result;
            var notificationTemplate = await _notificationDbContext.NotificationTemplates.FirstOrDefaultAsync(x =>
                x.NotificationEventId.Equals(eventName));
            if (notificationTemplate == null)
            {
                await _notificationDbContext.NotificationTemplates.AddAsync(new NotificationTemplate
                {
                    NotificationEventId = eventName,
                    Subject = subject,
                    Value = template
                });
            }
            else
            {
                notificationTemplate.Value = template;
                notificationTemplate.Subject = subject;
                _notificationDbContext.Update(notificationTemplate);
            }

            await _notificationDbContext.PushAsync();

            var subscriptions =
                await _notificationDbContext.NotificationSubscriptions.Where(x =>
                    x.NotificationEventId.Equals(eventName)).ToListAsync();
            if (subscriptions.Any())
            {
                _notificationDbContext.NotificationSubscriptions.RemoveRange(subscriptions);
            }

            var dataRoles = roles?.ToList();
            if (dataRoles != null && dataRoles.Any())
            {
                var newSubscriptions = dataRoles.Select(x => new NotificationSubscription
                {
                    RoleId = x,
                    NotificationEventId = eventName
                });
                await _notificationDbContext.NotificationSubscriptions.AddRangeAsync(newSubscriptions);
            }

            return await _notificationDbContext.PushAsync();
        }
    }
}
