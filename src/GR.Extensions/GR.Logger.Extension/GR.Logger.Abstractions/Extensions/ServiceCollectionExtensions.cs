using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Logger.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register logger module
        /// </summary>
        /// <typeparam name="TLoggerFactory"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterLoggerModule<TLoggerFactory>(this IServiceCollection services)
            where TLoggerFactory : class, IGearLoggerFactory
        {
            services.AddGearSingleton<IGearLoggerFactory, TLoggerFactory>();
            return services;
        }
    }
}
