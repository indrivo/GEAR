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
            CalendarEvents.SystemCalendarEvents.OnEventCreated += async (sender, args) =>
            {
                //TODO: check user preferences
                return;
                var service = IoC.Resolve<ICalendarManager>();
                var evtRequest = await service.GetEventByIdAsync(args.EventId);
                if (!evtRequest.IsSuccess) return;
                var evt = evtRequest.Result;
                var factory = new ExternalCalendarProviderFactory();
                var providers = factory.GetProviders();
                foreach (var provider in providers)
                {
                    var providerService = factory.CreateService(provider);
                    var authRequest = await providerService.AuthorizeAsync(evt.Organizer);
                    if (!authRequest.IsSuccess) continue;
                    var syncResult = await providerService.PushEventAsync(EventMapper.Map(evt));
                    if (!syncResult.IsSuccess)
                    {
                        Debug.WriteLine(syncResult.Errors);
                    }
                }
            };

            return serviceCollection;
        }
    }
}
