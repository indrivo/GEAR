using GR.Core;
using GR.Core.Extensions;
using GR.Identity.Clients.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Clients.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add api clients razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiClientsRazorModule(this IServiceCollection services)
        {
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    var service = x.InjectService<IMenuService>();
                    await service.AppendMenuItemsAsync(new ClientsMenuInitializer());
                });
            };
            return services;
        }
    }
}
