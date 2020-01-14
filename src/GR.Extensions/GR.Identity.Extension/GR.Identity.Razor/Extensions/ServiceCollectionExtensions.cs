using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add identity razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityRazorModule(this IServiceCollection services)
        {
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
           {
               GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
               {
                   var service = IoC.Resolve<IMenuService>();
                   await service.AppendMenuItemsAsync(new IdentityMenuInitializer());
               });
           };
            return services;
        }
    }
}
