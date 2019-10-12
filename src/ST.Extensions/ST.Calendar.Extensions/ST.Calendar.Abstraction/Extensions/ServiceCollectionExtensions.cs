using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Calendar.Abstractions.Events;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
using ST.Core.Extensions;
using ST.Core.Helpers;


namespace ST.Calendar.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register calendar module
        /// </summary>
        /// <typeparam name="TCalendarService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarModule<TCalendarService>(this IServiceCollection services)
            where TCalendarService : class, ICalendarManager
        {
            Arg.NotNull(services, nameof(AddCalendarModule));
            IoC.RegisterTransientService<ICalendarManager, TCalendarService>();
            return new CalendarServiceCollection(services);
        }

        /// <summary>
        /// Add calendar module storage
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarModuleStorage<TDbContext>(
            this CalendarServiceCollection configuration, Action<DbContextOptionsBuilder> options)
            where TDbContext : DbContext, ICalendarDbContext
        {
            Arg.NotNull(configuration.Services, nameof(AddCalendarModuleStorage));
            configuration.Services.AddDbContext<TDbContext>(options, ServiceLifetime.Transient);
            configuration.Services.AddScopedContextFactory<ICalendarDbContext, TDbContext>();
            return configuration;
        }


        /// <summary>
        /// Register runtime events
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection AddCalendarRuntimeEvents(this CalendarServiceCollection serviceCollection)
        {
            CalendarEvents.RegisterEvents();
            return serviceCollection;
        }

        /// <summary>
        /// Register token provider
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterTokenProvider<TProvider>(this CalendarServiceCollection serviceCollection)
            where TProvider : class, ICalendarExternalTokenProvider
        {
            IoC.RegisterTransientService<ICalendarExternalTokenProvider, TProvider>();
            return serviceCollection;
        }

        /// <summary>
        /// Register calendar
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static CalendarServiceCollection RegisterCalendarUserPreferencesProvider<TService>(this CalendarServiceCollection serviceCollection)
            where TService : class, ICalendarUserSettingsService
        {
            IoC.RegisterTransientService<ICalendarUserSettingsService, TService>();
            return serviceCollection;
        }
    }
}
