using Microsoft.Extensions.DependencyInjection;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Identity.Services;

namespace ST.Identity.Extensions
{
    public static class PermissionExtension
    {
        /// <summary>
        /// Add Authorization based on cache
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationBasedOnCache<TContext>(this IServiceCollection services) where TContext : ApplicationDbContext
        {
            services.AddTransient<IPermissionService, PermissionService<TContext>>();
            return services;
        }
    }
}
