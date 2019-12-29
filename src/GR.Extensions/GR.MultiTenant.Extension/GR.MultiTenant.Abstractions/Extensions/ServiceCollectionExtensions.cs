using Microsoft.Extensions.DependencyInjection;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions.Events;

namespace GR.MultiTenant.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register module
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTenantModule<TRepository, TTenant>(this IServiceCollection services)
            where TTenant : Tenant
            where TRepository : class, IOrganizationService<TTenant>
        {
            services.AddTransient<IOrganizationService<TTenant>, TRepository>();
            TenantEvents.RegisterEvents();
            return services;
        }
    }
}