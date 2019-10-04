using Microsoft.Extensions.DependencyInjection;
using ST.Calendar.Abstractions.Helpers.ServiceBuilders;
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
        public static CalendarServiceCollection AddCalendarRazorUIModule(this CalendarServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(InternalCalendarFileConfiguration));
            return services;
        }
    }
}
