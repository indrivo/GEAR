using GR.Core;
using Microsoft.Extensions.DependencyInjection;
using GR.Report.Dynamic.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;

namespace GR.Report.Dynamic.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddReportUiModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(ReportFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (s, c) =>
                {
                    await s.GetService<IMenuService>().AppendMenuItemsAsync(new ReportsMenuInitializer());
                });
            };
            return services;
        }
    }
}
