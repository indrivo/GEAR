using Microsoft.Extensions.DependencyInjection;

namespace ST.Install.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register install module
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInstallerModule<TService>(this IServiceCollection services) where TService : class, ISyncInstaller
        {
            services.AddTransient<ISyncInstaller, TService>();
            return services;
        }
    }
}
