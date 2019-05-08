using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ST.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Register system config
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSystemConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SystemConfig>(configuration.GetSection(nameof(SystemConfig)));
            return services;
        }
    }
}
