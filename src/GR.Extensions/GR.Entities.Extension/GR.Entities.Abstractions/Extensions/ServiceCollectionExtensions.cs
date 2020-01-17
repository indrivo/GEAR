using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core.Events;
using GR.Core.Events.EventArgs;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Events;
using GR.Entities.Abstractions.Helpers;
using GR.Entities.Abstractions.Query;

namespace GR.Entities.Abstractions.Extensions
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
            where TEntityRepository : class, IEntityService
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<IEntityContext, TEntityContext>();
            IoC.RegisterTransientService<IEntityContext, TEntityContext>();

            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(IEntityService), typeof(TEntityRepository) }
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

            SystemEvents.Application.OnApplicationStarted += delegate (object sender, ApplicationStartedEventArgs args)
            {
                var scopeContextFactory = (DbContext)args.Services.GetRequiredService<IEntityContext>();
                DbConnectionFactory.Connection.SetConnection(scopeContextFactory.Database.GetDbConnection());
            };

            SystemEvents.Application.OnApplicationStopped += delegate
            {
                DbConnectionFactory.CloseAll();
            };

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

        /// <summary>
        /// Add entity module storage
        /// </summary>
        /// <typeparam name="TEntityContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityModuleStorage<TEntityContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TEntityContext : DbContext, IEntityContext
        {
            services.RegisterAuditFor<IEntityContext>("Entity module");
            services.AddDbContext<TEntityContext>(options, ServiceLifetime.Transient);
            return services;
        }
    }
}
