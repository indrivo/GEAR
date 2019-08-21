using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Security.Abstractions.ServiceBuilder;

namespace ST.Entities.Security.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register services
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IEntitySecurityServiceCollection AddEntityRoleAccessModule<TRepository>(this IServiceCollection services)
            where TRepository : class, IEntityRoleAccessManager
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<IEntityRoleAccessManager, TRepository>();
            IoC.RegisterService<IEntityRoleAccessManager, TRepository>();
            return new EntitySecurityServiceCollection(services);
        }

        /// <summary>
        /// Add storage configuration
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IEntitySecurityServiceCollection AddEntityModuleSecurityStorage<TContext>(
            this IEntitySecurityServiceCollection configuration, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IEntitySecurityDbContext
        {
            configuration.Services.AddDbContext<TContext>(options);
            configuration.Services.AddScopedContextFactory<IEntitySecurityDbContext, TContext>();
            return configuration;
        }
    }
}
