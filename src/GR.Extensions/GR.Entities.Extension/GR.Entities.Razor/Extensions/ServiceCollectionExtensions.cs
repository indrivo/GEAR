using GR.Core;
using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GR.Entities.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Entities.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(EntityRazorFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    var service = x.InjectService<IMenuService>();
                    await service.AppendMenuItemsAsync(new EntitiesMenuInitializer());
                });
            };
            return services;
        }
    }
}
