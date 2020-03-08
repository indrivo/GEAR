using System;
using System.Diagnostics;
using GR.Calendar.Abstractions.Events;
using GR.Calendar.Abstractions.ExternalProviders.Exceptions;
using GR.Calendar.Abstractions.ExternalProviders.Helpers;
using GR.Calendar.Abstractions.Helpers.Mappers;
using GR.Core.Helpers;
using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Calendar.Abstractions.ExternalProviders.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register external calendar provider
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterExternalCalendarProvider(this IServiceCollection serviceCollection, Action<ExternalProviderConfig> options)
        {
            var configuration = new ExternalProviderConfig();
            options(configuration);
            if (configuration.ProviderName.IsNullOrEmpty() || configuration.ProviderType == null)
                throw new FailRegisterProviderException();
            IoC.RegisterService<IExternalCalendarProvider>(configuration.ProviderName, configuration.ProviderType);
            CalendarProviders.RegisterProviderInMemory(configuration);
            return serviceCollection;
        }

        /// <summary>
        /// Register events
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSyncOnExternalCalendars(this IServiceCollection serviceCollection)
        {
            //On event created
            CalendarEvents.SystemCalendarEvents.OnEventCreated += async (sender, args) =>
            {
                var calendarManager = IoC.Resolve<ICalendarManager>();
                var userSettingsService = IoC.Resolve<ICalendarUserSettingsService>();
                var evtRequest = await calendarManager.GetEventByIdAsync(args.EventId);
                if (!evtRequest.IsSuccess) return;
                var evt = evtRequest.Result;
                var factory = new ExternalCalendarProviderFactory();
                var providers = factory.GetProviders();
                foreach (var provider in providers)
                {
                    var isProviderEnabledForUser = await userSettingsService.IsProviderEnabledAsync(evt.Organizer, provider);
                    if (!isProviderEnabledForUser.IsSuccess) continue;
                    var providerService = factory.CreateService(provider);
                    var authRequest = await providerService.AuthorizeAsync(evt.Organizer);
                    if (!authRequest.IsSuccess) continue;
                    var syncResult = await providerService.PushEventAsync(EventMapper.Map(evt));
                    if (!syncResult.IsSuccess)
                    {
                        Debug.WriteLine(syncResult.Errors);
                    }
                }

                await calendarManager.SetEventSyncState(evt.Id, true);
            };

            //On event update
            CalendarEvents.SystemCalendarEvents.OnEventUpdated += async (sender, args) =>
            {
                var calendarManager = IoC.Resolve<ICalendarManager>();
                var userSettingsService = IoC.Resolve<ICalendarUserSettingsService>();
                var evtRequest = await calendarManager.GetEventByIdAsync(args.EventId);
                if (!evtRequest.IsSuccess) return;
                var evt = evtRequest.Result;
                var factory = new ExternalCalendarProviderFactory();
                var providers = factory.GetProviders();
                foreach (var provider in providers)
                {
                    var isProviderEnabledForUser = await userSettingsService.IsProviderEnabledAsync(evt.Organizer, provider);
                    if (!isProviderEnabledForUser.IsSuccess) continue;
                    var providerService = factory.CreateService(provider);
                    var authRequest = await providerService.AuthorizeAsync(evt.Organizer);
                    if (!authRequest.IsSuccess) continue;

                    if (!evt.Synced) await providerService.PushEventAsync(EventMapper.Map(evt));
                    else
                    {
                        var attrRequest = await userSettingsService.GetEventAttributeAsync(evt.Id, $"{provider}_evtId");
                        if (attrRequest.IsSuccess)
                        {
                            var providerEventId = attrRequest.Result;
                            var syncResult = await providerService.UpdateEventAsync(EventMapper.Map(evt), providerEventId);
                            if (!syncResult.IsSuccess)
                            {
                                Debug.WriteLine(syncResult.Errors);
                            }
                        }
                        else
                        {
                            await providerService.PushEventAsync(EventMapper.Map(evt));
                        }
                    }
                }

                if (!evt.Synced) await calendarManager.SetEventSyncState(evt.Id, true);
            };

            //On delete Event
            CalendarEvents.SystemCalendarEvents.OnEventDeleted += async (sender, args) =>
            {
                var calendarManager = IoC.Resolve<ICalendarManager>();
                var userSettingsService = IoC.Resolve<ICalendarUserSettingsService>();
                var evtRequest = await calendarManager.GetEventByIdAsync(args.EventId);
                if (!evtRequest.IsSuccess) return;
                var evt = evtRequest.Result;
                if (!evt.Synced) return;
                var factory = new ExternalCalendarProviderFactory();
                var providers = factory.GetProviders();

                foreach (var provider in providers)
                {
                    var isProviderEnabledForUser = await userSettingsService.IsProviderEnabledAsync(evt.Organizer, provider);
                    if (!isProviderEnabledForUser.IsSuccess) continue;
                    var providerService = factory.CreateService(provider);
                    var authRequest = await providerService.AuthorizeAsync(evt.Organizer);
                    if (!authRequest.IsSuccess) continue;
                    var attrRequest = await userSettingsService.GetEventAttributeAsync(evt.Id, $"{provider}_evtId");
                    if (!attrRequest.IsSuccess) continue;
                    var providerEventId = attrRequest.Result;
                    await providerService.DeleteEventAsync(providerEventId);
                }
            };

            return serviceCollection;
        }
    }
}
