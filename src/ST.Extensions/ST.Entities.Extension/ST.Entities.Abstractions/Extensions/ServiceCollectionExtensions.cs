using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Events;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Events;
using ST.Entities.Abstractions.Query;

namespace ST.Entities.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add entity module
        /// </summary>
        /// <typeparam name="TEntityContext"></typeparam>
        /// <typeparam name="TEntityRepository"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityModule<TEntityContext, TEntityRepository>(this IServiceCollection services)
            where TEntityContext : DbContext, IEntityContext
            where TEntityRepository : class, IEntityRepository
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<IEntityContext, TEntityContext>();

            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(IEntityContext), typeof(TEntityContext) },
                { typeof(IEntityRepository), typeof(TEntityRepository) }
            });
            return services;
        }

        /// <summary>
        /// Register entity events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityModuleEvents(this IServiceCollection services)
        {
            //Register entity events
            EntityEvents.RegisterEvents();
            return services;
        }

        /// <summary>
        /// Add entity query builders
        /// </summary>
        /// <typeparam name="TQueryTableBuilder"></typeparam>
        /// <typeparam name="TEntityQueryBuilder"></typeparam>
        /// <typeparam name="TTablesService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityModuleQueryBuilders<TQueryTableBuilder, TEntityQueryBuilder, TTablesService>(this IServiceCollection services)
            where TQueryTableBuilder : class, IQueryTableBuilder
            where TEntityQueryBuilder : class, IEntityQueryBuilder
            where TTablesService : class, ITablesService
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<ITablesService, TTablesService>();
            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(IQueryTableBuilder), typeof(TQueryTableBuilder) },
                { typeof(IEntityQueryBuilder), typeof(TEntityQueryBuilder) },
                { typeof(ITablesService), typeof(TTablesService) }
            });
            return services;
        }

        public static IServiceCollection AddEntityModuleStorage<TEntityContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TEntityContext : DbContext, IEntityContext
        {
            services.AddDbContext<TEntityContext>(options, ServiceLifetime.Transient);
            return services;
        }
    }
}
