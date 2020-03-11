using GR.Core;
using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using GR.Dashboard.Abstractions.ServiceBuilder;
using GR.Dashboard.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Dashboard.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IDashboardServiceCollection AddDashboardRazorUIModule(this IDashboardServiceCollection services)
        {
            services.Services.ConfigureOptions(typeof(DashBoardFileConfiguration));

            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    await x.InjectService<IMenuService>().AppendMenuItemsAsync(new DashboardMenuInitializer());
                });
            };
            return services;
        }
    }
}
