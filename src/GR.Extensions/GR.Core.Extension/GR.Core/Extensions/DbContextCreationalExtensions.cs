using System;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Core.Extensions
{
    public static class DbContextCreatableExtensions
    {
        /// <summary>
        /// Add scoped context factory
        /// </summary>
        /// <typeparam name="TIContext"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedContextFactory<TIContext, TContext>(this IServiceCollection services)
            where TContext : DbContext, TIContext
            where TIContext : class, IDbContext
        {
            if (!typeof(TIContext).IsInterface)
                throw new Exception($"{nameof(TIContext)} must be an interface in extension {nameof(AddScopedContextFactory)}");

            TIContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TContext>();
                IoC.RegisterScopedService<TIContext, TContext>(context);
                return context;
            }

            services.AddScoped(ContextFactory);
            return services;
        }


        /// <summary>
        /// Add scoped context factory
        /// </summary>
        /// <typeparam name="TIContext"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTransientContextFactory<TIContext, TContext>(this IServiceCollection services)
            where TContext : DbContext, TIContext
            where TIContext : class, IDbContext
        {
            if (!typeof(TIContext).IsInterface)
                throw new Exception($"{nameof(TIContext)} must be an interface in extension {nameof(AddTransientContextFactory)}");

            TIContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TContext>();
                IoC.RegisterTransientService<TIContext, TContext>(context);
                return context;
            }

            services.AddTransient(ContextFactory);
            return services;
        }
    }
}
