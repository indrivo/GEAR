using System;
using GR.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// Add logging configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services,
            Action<ILoggingBuilder> configuration)
        {
            services.AddLogging(configuration);
            return services;
        }
    }
}
