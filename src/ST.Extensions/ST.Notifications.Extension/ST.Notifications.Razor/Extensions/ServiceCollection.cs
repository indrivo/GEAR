using Microsoft.Extensions.DependencyInjection;
using ST.Notifications.Abstractions.ServiceBuilder;
using ST.Notifications.Razor.Helpers;

namespace ST.Notifications.Razor.Extensions
{
    public static class ServiceCollection
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static INotificationSubscriptionServiceCollection AddNotificationUiModule(this INotificationSubscriptionServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(NotificationRazorFileConfiguration));
            return services;
        }
    }
}
