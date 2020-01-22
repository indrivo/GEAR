using System;
using GR.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Add singleton service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGearSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<TService, TImplementation>();
            IoC.RegisterSingletonService<TService, TImplementation>();
            return services;
        }

        /// <summary>
        /// Add transient service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGearTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddTransient<TService, TImplementation>();
            IoC.RegisterTransientService<TService, TImplementation>();
            return services;
        }

        /// <summary>
        /// Add scoped service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGearScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<TService, TImplementation>();
            IoC.RegisterScopedService<TService, TImplementation>();

            return services;
        }

        /// <summary>
        /// Inject 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="_"></param>
        /// <returns></returns>
        public static TService InjectService<TService>(this object _)
            => IoC.Resolve<TService>();
    }
}
