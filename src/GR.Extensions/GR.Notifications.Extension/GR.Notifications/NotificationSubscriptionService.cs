﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Data;
using Microsoft.Extensions.Caching.Memory;

namespace GR.Notifications
{
    public class NotificationSubscriptionService : INotificationSubscriptionService
    {
        #region Injectable

        /// <summary>
        /// Inject role manager
        /// </summary>
        private readonly RoleManager<GearRole> _roleManager;

        /// <summary>
        /// Inject notification db context
        /// </summary>
        private readonly INotificationSubscriptionsDbContext _notificationSubscriptionsDbContext;

        /// <summary>
        /// Inject memory cache
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        #endregion

        public NotificationSubscriptionService(RoleManager<GearRole> roleManager, INotificationSubscriptionsDbContext notificationSubscriptionsDbContext, IMemoryCache memoryCache)
        {
            _roleManager = roleManager;
            _notificationSubscriptionsDbContext = notificationSubscriptionsDbContext;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Roles
        /// </summary>
        public IEnumerable<GearRole> Roles => _roleManager.Roles.ToList();

        /// <summary>
        /// Events
        /// </summary>
        public IEnumerable<NotificationEvent> Events => _notificationSubscriptionsDbContext.NotificationEvents.ToList();

        /// <summary>
        /// Get template event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<NotificationTemplate>> GetEventTemplateAsync(string eventName)
        {
            Arg.NotNullOrEmpty(eventName, nameof(GetRolesSubscribedToEventAsync));
            var key = GenerateEventTemplateCacheKey(eventName);
            var templateFromCache = _memoryCache.Get<NotificationTemplate>(key);
            if (templateFromCache != null) return new SuccessResultModel<NotificationTemplate>(templateFromCache);
            var template = await _notificationSubscriptionsDbContext.NotificationTemplates.FirstOrDefaultAsync(x =>
                    x.NotificationEventId.Equals(eventName));
            if (template == null) return new NotFoundResultModel<NotificationTemplate>();
            _memoryCache.Set(key, template);
            return new SuccessResultModel<NotificationTemplate>(template);
        }

        /// <summary>
        /// Get subscribed roles to event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GearRole>>> GetRolesSubscribedToEventAsync(string eventName)
        {
            Arg.NotNullOrEmpty(eventName, nameof(GetRolesSubscribedToEventAsync));
            var key = GenerateEventCacheKey(eventName);
            var cachedRoles = _memoryCache.Get<IList<GearRole>>(key);
            if (cachedRoles != null && !cachedRoles.Any()) return new NotFoundResultModel<IEnumerable<GearRole>>();
            if (cachedRoles != null && cachedRoles.Any()) return new SuccessResultModel<IEnumerable<GearRole>>(cachedRoles);

            var result = new ResultModel<IEnumerable<GearRole>>();
            var subscribed = await _notificationSubscriptionsDbContext.NotificationSubscriptions
                .Where(x => x.NotificationEventId.Equals(eventName)).ToListAsync();
            if (!subscribed.Any())
            {
                _memoryCache.Set(key, new List<GearRole>());
                return result;
            }
            var roles = await _roleManager.Roles.Where(x => subscribed.Select(j => j.RoleId).Contains(x.Id))
                .ToListAsync();
            result.IsSuccess = true;
            result.Result = roles;
            _memoryCache.Set(key, roles);
            return result;
        }

        /// <summary>
        /// Seed events
        /// </summary>
        /// <returns></returns>
        public virtual async Task SeedEventsAsync()
        {
            var events = GetSystemEvents();
            var dbEvents = await _notificationSubscriptionsDbContext.NotificationEvents.ToListAsync();
            var nonSeeded = events.Where(x => !dbEvents.Select(v => v.EventId).Contains(x.EventId));
            await _notificationSubscriptionsDbContext.NotificationEvents.AddRangeAsync(nonSeeded);
            await _notificationSubscriptionsDbContext.PushAsync();
        }

        /// <summary>
        /// Get all system not seeded events
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<NotificationEvent> GetSystemEvents()
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
        public virtual async Task<ResultModel> SubscribeRolesToEventAsync(IEnumerable<Guid> roles, string eventName, string template, string subject)
        {
            var result = new ResultModel();
            if (string.IsNullOrEmpty(eventName)) return result;
            if (!SystemEvents.Common.HasEvent(eventName)) return result;
            var notificationTemplate = await _notificationSubscriptionsDbContext.NotificationTemplates.FirstOrDefaultAsync(x =>
                x.NotificationEventId.Equals(eventName));
            if (notificationTemplate == null)
            {
                await _notificationSubscriptionsDbContext.NotificationTemplates.AddAsync(new NotificationTemplate
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
                _notificationSubscriptionsDbContext.Update(notificationTemplate);
            }

            await _notificationSubscriptionsDbContext.PushAsync();

            var subscriptions =
                await _notificationSubscriptionsDbContext.NotificationSubscriptions.Where(x =>
                    x.NotificationEventId.Equals(eventName)).ToListAsync();
            if (subscriptions.Any())
            {
                _notificationSubscriptionsDbContext.NotificationSubscriptions.RemoveRange(subscriptions);
            }

            var dataRoles = roles?.ToList();
            if (dataRoles != null && dataRoles.Any())
            {
                var newSubscriptions = dataRoles.Select(x => new NotificationSubscription
                {
                    RoleId = x,
                    NotificationEventId = eventName
                });
                await _notificationSubscriptionsDbContext.NotificationSubscriptions.AddRangeAsync(newSubscriptions);
            }

            var eventRolesKey = GenerateEventCacheKey(eventName);
            var templateKey = GenerateEventTemplateCacheKey(eventName);
            _memoryCache.Remove(eventRolesKey);
            _memoryCache.Remove(templateKey);
            return await _notificationSubscriptionsDbContext.PushAsync();
        }

        #region Helpers

        /// <summary>
        /// Generate cache key for subscribed roles
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        protected static string GenerateEventCacheKey(string eventName) => $"subscribed_roles_for_{eventName}_event";

        /// <summary>
        /// Generate cache key for event template
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        protected static string GenerateEventTemplateCacheKey(string eventName) => $"template_for_{eventName}_event";

        #endregion
    }
}
