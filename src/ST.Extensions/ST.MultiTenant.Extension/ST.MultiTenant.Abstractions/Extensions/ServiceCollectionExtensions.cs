using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using ST.Identity.Abstractions.Models.MultiTenants;

namespace ST.MultiTenant.Abstractions.Extensions
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
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                    .ActionContext;
                return new UrlHelper(actionContext);
            });
            return services;
        }
    }
}