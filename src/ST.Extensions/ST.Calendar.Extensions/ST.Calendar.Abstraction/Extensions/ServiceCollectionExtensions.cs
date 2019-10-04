using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
