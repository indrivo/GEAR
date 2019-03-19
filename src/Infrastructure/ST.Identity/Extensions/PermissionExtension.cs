using Microsoft.Extensions.DependencyInjection;
using ST.Identity.Abstractions;
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
        public static IServiceCollection AddAuthorizationBasedOnCache(this IServiceCollection services)
        {
            services.AddTransient<IPermissionService, PermissionService>();
            return services;
        }
    }
}
