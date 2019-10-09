using System;
using System.Diagnostics;
using System.Linq;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Events;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Calendar.Abstractions.ExternalProviders.Extensions;
using ST.Calendar.Abstractions.Helpers.Mappers;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Helpers;

namespace ST.Calendar.Providers.Google.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register google calendar
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterGoogleCalendarProvider(this CalendarServiceCollection serviceCollection)
        {
            serviceCollection.RegisterExternalCalendarProvider(options =>
            {
                options.ProviderName = nameof(GoogleCalendarProvider);
                options.ProviderType = typeof(GoogleCalendarProvider);
            });

            return serviceCollection;
        }
    }
}
