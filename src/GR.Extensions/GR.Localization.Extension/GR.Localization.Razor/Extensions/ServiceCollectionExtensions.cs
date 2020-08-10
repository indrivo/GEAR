using GR.Core;
using GR.Localization.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Localization.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add localization razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationRazorModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(LocalizationRazorFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (serviceProvider, cancellationToken) =>
                {
                    var service = serviceProvider.GetService<IMenuService>();
                    await service.AppendMenuItemsAsync(new LocalizationMenuInitializer());
                });
            };
            return services;
        }
    }
}
