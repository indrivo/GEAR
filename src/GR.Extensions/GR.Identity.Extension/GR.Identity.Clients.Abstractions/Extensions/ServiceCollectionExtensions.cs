using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Configurations;
using GR.Identity.Abstractions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IProfileService = IdentityServer4.Services.IProfileService;

namespace GR.Identity.Clients.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add identity server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="migrationsAssembly"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityClientsModule<TUser>(this IServiceCollection services, IConfiguration configuration,
            string migrationsAssembly)
            where TUser : GearUser
        {
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<TUser>()
                .AddConfigurationStore(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = builder => builder.RegisterIdentityStorage(configuration, migrationsAssembly);
                })
                .AddOperationalStore(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = builder => builder.RegisterIdentityStorage(configuration, migrationsAssembly);
                });

            return services;
        }

        /// <summary>
        /// Add profile services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientsProfileService<TProfileService>(this IServiceCollection services)
            where TProfileService : class, IProfileService
        {
            services.AddTransient<IProfileService, TProfileService>();
            return services;
        }


        /// <summary>
        /// Add clients service
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterClientsService<TService>(this IServiceCollection services)
            where TService : class, IClientsService
        {
            services.AddGearTransient<IClientsService, TService>();
            return services;
        }
    }
}