using GR.Core;
using GR.Core.Extensions;
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
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    var service = x.InjectService<IMenuService>();
                    await service.AppendMenuItemsAsync(new LocalizationMenuInitializer());
                });
            };
            return services;
        }
    }
}
