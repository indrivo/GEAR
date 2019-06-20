using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ST.DynamicEntityStorage.Abstractions;
using ST.PageRender.Abstractions;
using ST.PageRender.Razor.Helpers;
using ST.PageRender.Razor.Services;

namespace ST.PageRender.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register page render
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageRenderUiModule(this IServiceCollection services)
        {
            services.AddTransient<IPageRender, Services.PageRender>();
            services.AddTransient<IMenuService, MenuService<IDynamicService>>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IPageAclService, PageAclService>();
            services.ConfigureOptions(typeof(PageRenderFileConfiguration));
            return services;
        }
    }
}
