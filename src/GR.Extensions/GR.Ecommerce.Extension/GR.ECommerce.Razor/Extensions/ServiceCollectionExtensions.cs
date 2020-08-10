using GR.Core;
using GR.ECommerce.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ECommerce.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register ui module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCommerceRazorUiModule(this IServiceCollection services)
        {
            services.ConfigureOptions(typeof(CommerceRazorFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (serviceProvider, cancellationToken) =>
                    {
                        await serviceProvider.GetService<IMenuService>().AppendMenuItemsAsync(new CommerceMenuInitializer());
                    });
            };
            services.AddScoped<ProductGalleryManager>();
            return services;
        }
    }
}
