using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;
using ST.PageRender.Abstractions.Events;

namespace ST.PageRender.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageModule<TPageContext>(this IServiceCollection services)
            where TPageContext : DbContext, IDynamicPagesContext
        {
            Arg.NotNull(services, nameof(services));
            DynamicUiEvents.RegisterEvents();
            IDynamicPagesContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TPageContext>();
                IoC.RegisterScopedService<IDynamicPagesContext, TPageContext>(context);
                return context;
            }

            services.AddScoped(ContextFactory);
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
            services.AddDbContext<TPageContext>(options);
            return services;
        }
    }
}
