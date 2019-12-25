using GR.PageRender.Abstractions;
using GR.PageRender.Razor.Helpers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.ConfigureOptions(typeof(PageRenderFileConfiguration));
            return services;
        }

        /// <summary>
        /// Menu service
        /// </summary>
        /// <typeparam name="TMenuService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMenuService<TMenuService>(this IServiceCollection services)
            where TMenuService : class, IMenuService
        {
            services.AddTransient<IMenuService, TMenuService>();
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