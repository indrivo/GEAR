using Microsoft.Extensions.DependencyInjection;
using ST.Configuration.Services.Abstraction;
using ST.DynamicEntityStorage.Abstractions;
using ST.PageRender.Razor.Helpers;
using ST.PageRender.Razor.Services;
using ST.PageRender.Razor.Services.Abstractions;

namespace ST.PageRender.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageRenderModule(this IServiceCollection services)
        {
            services.AddTransient<IPageRender, Services.PageRender>();
            services.AddTransient<IMenuService, MenuService<IDynamicService>>();
            services.ConfigureOptions(typeof(PageRenderFileConfiguration));
            return services;
        }
    }
}
