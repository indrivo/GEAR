using GR.Core;
using GR.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using GR.TaskManager.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.TaskManager.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTaskManagerRazorUIModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(TaskManagerRazorFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
           {
               GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
               {
                   var service = IoC.Resolve<IMenuService>();
                   await service.AppendMenuItemsAsync(new TaskManagerMenuInitializer());
               });
           };
            return services;
        }
    }
}
