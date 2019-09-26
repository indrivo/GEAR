using Microsoft.Extensions.DependencyInjection;
using ST.Calendar.Razor.Helpers;

namespace ST.Calendar.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInternalCalendarModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(InternalCalendarFileConfiguration));
            return services;
        }
    }
}
