using Microsoft.Extensions.DependencyInjection;
using GR.Notifications.Abstractions.ServiceBuilder;
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
        public static INotificationSubscriptionServiceCollection AddNotificationRazorUIModule(this INotificationSubscriptionServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(NotificationRazorFileConfiguration));
            return services;
        }
    }
}
