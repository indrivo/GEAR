using System;
using IdentityServer4.Extensions;
using ST.Calendar.Abstractions.ExternalProviders.Exceptions;
using ST.Calendar.Abstractions.ExternalProviders.Helpers;
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
    }
}
