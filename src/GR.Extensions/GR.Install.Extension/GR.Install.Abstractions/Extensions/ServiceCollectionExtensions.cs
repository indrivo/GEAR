using Microsoft.Extensions.DependencyInjection;

namespace GR.Install.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register install module
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInstallerModule(this IServiceCollection services)
        {
            return services;
        }
    }
}
