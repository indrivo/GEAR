using GR.Core;
using GR.Core.Extensions;
using GR.Notifications.Subscriptions.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.Subscriptions.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddNotificationSubscriptionRazorUiModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(NotificationSubscriptionsRazorFileConfiguration));

            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    await x.InjectService<IMenuService>()
                        .AppendMenuItemsAsync(new NotificationsMenuInitializer());
                });
            };
            return services;
        }
    }
}
