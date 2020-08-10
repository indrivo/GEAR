using System;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Events;
using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions.Extensions
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
            services.AddGearScoped<ICommerceContext, TCommerceContext>();
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
            where TProductRepository : class, IProductService<TProduct>
            where TProduct : Product
        {
            Arg.NotNull(services, nameof(services));
            services.AddGearScoped<IProductService<TProduct>, TProductRepository>();
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
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost().MigrateDbContext<TContext>();
            };
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

        /// <summary>
        /// Register cart service
        /// </summary>
        /// <typeparam name="TCartService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCartService<TCartService>(this IServiceCollection services)
            where TCartService : class, ICartService
        {
            services.AddGearScoped<ICartService, TCartService>();
            return services;
        }

        /// <summary>
        /// Register brand service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterBrandService<TService>(this IServiceCollection services)
            where TService : class, IBrandsService
        {
            services.AddGearScoped<IBrandsService, TService>();
            return services;
        }

        /// <summary>
        /// Register product attribute service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterProductAttributeService<TService>(this IServiceCollection services)
            where TService : class, IProductAttributeService
        {
            services.AddGearScoped<IProductAttributeService, TService>();
            return services;
        }

        /// <summary>
        /// Register product type service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterProductTypeService<TService>(this IServiceCollection services)
            where TService : class, IProductTypeService
        {
            services.AddGearScoped<IProductTypeService, TService>();
            return services;
        }

        /// <summary>
        /// Register product category service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterProductCategoryService<TService>(this IServiceCollection services)
            where TService : class, IProductCategoryService
        {
            services.AddGearScoped<IProductCategoryService, TService>();
            return services;
        }
    }
}