using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddCalendarModule<TCalendarService>(this IServiceCollection services)
            where TCalendarService : class, ICalendarManager
        {
            services.AddTransient<ICalendarManager, TCalendarService>();
            IoC.RegisterService<ICalendarManager, TCalendarService>();
            return services;
        }
    }
}
