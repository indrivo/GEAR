using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using GR.DynamicEntityStorage.Abstractions;
using GR.PageRender.Abstractions;
using GR.PageRender.Razor.Helpers;
using GR.PageRender.Razor.Services;

namespace GR.PageRender.Razor.Extensions
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
