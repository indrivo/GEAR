using GR.Core;
using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GR.Entities.Security.Abstractions.ServiceBuilder;
using GR.Entities.Security.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Entities.Security.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEntitySecurityServiceCollection AddEntitySecurityRazorUIModule(this IEntitySecurityServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(EntitySecurityRazorFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                    {
                        await x.InjectService<IMenuService>().AppendMenuItemsAsync(new EntitiesPermissionsMenuInitializer());
                    });
            };

            return services;
        }
    }
}
