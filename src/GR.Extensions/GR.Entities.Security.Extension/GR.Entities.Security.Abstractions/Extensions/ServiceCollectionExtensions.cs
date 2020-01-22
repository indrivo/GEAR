using System;
using System.Threading.Tasks;
using GR.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Events;
using GR.Entities.Security.Abstractions.ServiceBuilder;

namespace GR.Entities.Security.Abstractions.Extensions
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
            where TRepository : class, IEntityRoleAccessService
        {
            Arg.NotNull(services, nameof(services));
            services.AddTransient<IEntityRoleAccessService, TRepository>();
            IoC.RegisterTransientService<IEntityRoleAccessService, TRepository>();

            EntityEvents.Entities.OnEntityDeleted += (sender, args) =>
            {
                var service = IoC.Resolve<IEntityRoleAccessService>();
                service.ClearEntityPermissionsFromCache(args.EntityId);
            };

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
