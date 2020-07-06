using Microsoft.Extensions.DependencyInjection;
using GR.Notifications.Razor.Helpers;

namespace GR.Notifications.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(NotificationRazorFileConfiguration));
            return services;
        }
    }
}
