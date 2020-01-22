using GR.Core;
using GR.Core.Extensions;
using GR.PageRender.Abstractions;
using GR.PageRender.Razor.Helpers;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace GR.PageRender.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register razor page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageRenderUIModule<TPageRenderService>(this IServiceCollection services)
            where TPageRenderService : class, IPageRender
        {
            services.AddTransient<IPageRender, TPageRenderService>();
            services.ConfigureOptions(typeof(PageRenderFileConfiguration));
            MenuEvents.Menu.OnMenuSeed += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                    {
                        await args.InjectService<IMenuService>().AppendMenuItemsAsync(new PagesMenuInitializer());
                    });
            };
            return services;
        }

        /// <summary>
        /// Add page acl
        /// </summary>
        /// <typeparam name="TPageAclService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageAclService<TPageAclService>(this IServiceCollection services)
            where TPageAclService : class, IPageAclService
        {
            services.AddTransient<IPageAclService, TPageAclService>();

            return services;
        }
    }
}