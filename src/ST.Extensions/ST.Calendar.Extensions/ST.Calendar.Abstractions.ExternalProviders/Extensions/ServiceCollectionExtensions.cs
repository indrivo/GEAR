using System;
using System.Diagnostics;
using IdentityServer4.Extensions;
using ST.Calendar.Abstractions.Events;
using ST.Calendar.Abstractions.ExternalProviders.Exceptions;
using ST.Calendar.Abstractions.ExternalProviders.Helpers;
using ST.Calendar.Abstractions.Helpers.Mappers;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions.ExternalProviders.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register external calendar provider
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterExternalCalendarProvider(this CalendarServiceCollection serviceCollection, Action<ExternalProviderConfig> options)
        {
            var configuration = new ExternalProviderConfig();
            options(configuration);
            if (configuration.ProviderName.IsNullOrEmpty() || configuration.ProviderType == null)
                throw new FailRegisterProviderException();
            IoC.RegisterService<IExternalCalendarProvider>(configuration.ProviderName, configuration.ProviderType);
            CalendarProviders.RegisterProviderInMemory(configuration.ProviderName);
            return serviceCollection;
        }

        /// <summary>
        /// Register events
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterSyncOnExternalCalendars(this CalendarServiceCollection serviceCollection)
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
