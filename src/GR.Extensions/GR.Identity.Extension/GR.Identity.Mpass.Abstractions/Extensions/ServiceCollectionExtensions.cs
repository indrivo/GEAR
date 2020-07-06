using GR.Core.Extensions;
using GR.Identity.Mpass.Abstractions.Security;
using GR.Identity.Mpass.Abstractions.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Mpass.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add <see cref="MPassSigningCredentials"/> service that will 
        /// provide the certificates to the MPass service.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static IServiceCollection AddMPassModuleSigningCredentials(this IServiceCollection services, MPassSigningCredentials credentials)
        {
            return services.AddSingleton<IMPassSigningCredentialsStore>(new DefaultMpassSigningCredentialsStore(credentials));
        }

        /// <summary>
        /// Add MPass service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMPassService<TService>(this IServiceCollection services)
            where TService : class, IMPassService
        {
            services.AddGearScoped<IMPassService, TService>();
            return services;
        }
    }
}
