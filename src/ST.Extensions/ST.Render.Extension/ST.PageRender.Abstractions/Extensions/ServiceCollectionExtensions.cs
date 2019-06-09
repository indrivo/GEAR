using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;

namespace ST.PageRender.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageModule<TPageContext>(this IServiceCollection services)
            where TPageContext : DbContext, IDynamicPagesContext
        {
            Arg.NotNull(services, nameof(services));
            IDynamicPagesContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TPageContext>();
                IoC.RegisterScopedService<IDynamicPagesContext, TPageContext>(context);
                return context;
            }

            services.AddScoped(ContextFactory);
            return services;
        }
    }
}
