using Microsoft.Extensions.DependencyInjection;
using ST.InternalCalendar.Razor.Helpers;

namespace ST.InternalCalendar.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInternalCalendar(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(InternalCalendarFileConfiguration));
            return services;
        }
    }
}
