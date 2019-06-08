using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Query;

namespace ST.Entities.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add entity model
        /// </summary>
        /// <typeparam name="TEntityContext"></typeparam>
        /// <typeparam name="TQueryTableBuilder"></typeparam>
        /// <typeparam name="TEntityQueryBuilder"></typeparam>
        /// <typeparam name="TTablesService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityModule<TEntityContext, TQueryTableBuilder, TEntityQueryBuilder, TTablesService>(this IServiceCollection services)
            where TEntityContext : DbContext, IEntityContext
            where TQueryTableBuilder : class, IQueryTableBuilder
            where TEntityQueryBuilder : class, IEntityQueryBuilder
            where TTablesService : class, ITablesService
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<IEntityContext, TEntityContext>();

            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(IEntityContext), typeof(TEntityContext) },
                { typeof(IQueryTableBuilder), typeof(TQueryTableBuilder) },
                { typeof(IEntityQueryBuilder), typeof(TEntityQueryBuilder) },
                { typeof(ITablesService), typeof(TTablesService) }
            });
            return services;
        }
    }
}
