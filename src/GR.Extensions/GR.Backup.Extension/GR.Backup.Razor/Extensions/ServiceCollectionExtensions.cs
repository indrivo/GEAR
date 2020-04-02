using GR.Backup.Razor.Helpers;
using GR.Core;
using GR.Core.Extensions;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Backup.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add backup razor module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBackupRazorModule(this IServiceCollection services)
        {
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    await x.InjectService<IMenuService>().AppendMenuItemsAsync(new BackupMenuInitializer());
                });
            };
            return services;
        }
    }
}
