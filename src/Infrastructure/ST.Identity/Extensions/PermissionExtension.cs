using Microsoft.AspNetCore.Hosting;
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
        /// <param name="environment"></param>
        /// <param name="ip"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorizationBasedOnCache(this IServiceCollection services, IHostingEnvironment environment, string ip = "127.0.0.1", string instance = "ST.CORE")
        {
            services.AddDistributedRedisCache(opts =>
            {
                opts.Configuration = ip;
                opts.InstanceName = $"{instance}.{environment.EnvironmentName}";
            });
            services.AddTransient<IPermissionService, PermissionService>();
            return services;
        }
    }
}
