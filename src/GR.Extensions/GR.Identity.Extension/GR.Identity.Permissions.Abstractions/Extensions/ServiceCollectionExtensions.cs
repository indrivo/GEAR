using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GR.Identity.Abstractions;

namespace GR.Identity.Permissions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Authorization based on cache
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationBasedOnCache<TContext, TPermissionService>(this IServiceCollection services)
            where TContext : DbContext, IIdentityContext
            where TPermissionService : class, IPermissionService
        {
            services.AddTransient<IPermissionService, TPermissionService>();
            return services;
        }
    }
}
