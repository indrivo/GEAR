﻿using Microsoft.Extensions.DependencyInjection;
using ST.Configuration.Services.Abstraction;
using ST.DynamicEntityStorage.Abstractions;
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
        public static IServiceCollection AddPageRender(this IServiceCollection services)
        {
            services.AddTransient<IPageRender, Configuration.Services.PageRender>();
            services.AddTransient<IMenuService, MenuService<IDynamicService>>();
            return services;
        }
    }
}
