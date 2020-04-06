using GR.Core;
using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GR.Notifications.Abstractions.ServiceBuilder;
using GR.Notifications.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Notifications.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static INotificationServiceCollection AddNotificationRazorUIModule(this INotificationServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(NotificationRazorFileConfiguration));

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
