using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;
using ST.ECommerce.Abstractions.Events;
using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register commerce module
        /// </summary>
        /// <typeparam name="TCommerceContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCommerceModule<TCommerceContext>(this IServiceCollection services) where TCommerceContext : DbContext, ICommerceContext
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<ICommerceContext, TCommerceContext>();
            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(ICommerceContext), typeof(TCommerceContext) },
            });
            return services;
        }

        /// <summary>
        /// Register product repository
        /// </summary>
        /// <typeparam name="TProductRepository"></typeparam>
        /// <typeparam name="TProduct"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCommerceProductRepository<TProductRepository, TProduct>(this IServiceCollection services)
            where TProductRepository : class, IProductRepository<TProduct>
            where TProduct : Product
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<IProductRepository<TProduct>, TProductRepository>();
            IoC.RegisterTransientService<IProductRepository<TProduct>, TProductRepository>();
            return services;
        }

        /// <summary>
        /// Register commerce storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="storageOptions"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCommerceStorage<TContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> storageOptions)
        where TContext : DbContext, ICommerceContext
        {
            services.AddDbContext<TContext>(storageOptions);
            return services;
        }

        /// <summary>
        /// Register service events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCommerceEvents(this IServiceCollection services)
        {
            //register commerce events
            CommerceEvents.RegisterEvents();
            return services;
        }
    }
}
