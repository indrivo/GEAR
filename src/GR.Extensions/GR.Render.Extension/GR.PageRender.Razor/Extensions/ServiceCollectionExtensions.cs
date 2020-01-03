using GR.PageRender.Abstractions;
using GR.PageRender.Razor.Helpers;
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