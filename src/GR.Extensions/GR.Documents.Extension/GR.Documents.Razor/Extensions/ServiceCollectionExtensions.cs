using Microsoft.Extensions.DependencyInjection;
using GR.Documents.Razor.Helpers;
using GR.UI.Menu.Abstractions.Events;
using GR.Core;
using GR.UI.Menu.Abstractions;

namespace GR.Documents.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDocumentRazorUiModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(DocumentRazorFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (s, c) =>
                {
                    await s.GetService<IMenuService>().AppendMenuItemsAsync(new DocumentMenuInitializer());
                });
            };
            return services;
        }

    }
}
