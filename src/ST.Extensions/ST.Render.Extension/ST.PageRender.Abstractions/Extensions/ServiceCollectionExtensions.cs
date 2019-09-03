using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Audit.Abstractions.Extensions;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.PageRender.Abstractions.Events;

namespace ST.PageRender.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add page module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageModule(this IServiceCollection services)
        {
            Arg.NotNull(services, nameof(services));
            DynamicUiEvents.RegisterEvents();
            return services;
        }

        /// <summary>
        /// Add module storage
        /// </summary>
        /// <typeparam name="TPageContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddPageModuleStorage<TPageContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TPageContext : DbContext, IDynamicPagesContext
        {
            services.AddScopedContextFactory<IDynamicPagesContext, TPageContext>();
            services.AddDbContext<TPageContext>(options);
            services.RegisterAuditFor<IDynamicPagesContext>("Page module");
            return services;
        }
    }
}
