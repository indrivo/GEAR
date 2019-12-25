using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Configurations;
using GR.Identity.Abstractions.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity_IProfileService = IdentityServer4.Services.IProfileService;
using Identity_ProfileService = GR.Identity.Services.ProfileService;

namespace GR.Identity.IdentityServer4.Extensions
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
        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration,
            string migrationsAssembly)
        {
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<GearUser>()
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
        public static IServiceCollection AddIdentityModuleProfileServices(this IServiceCollection services)
        {
            services.AddTransient<Identity_IProfileService, Identity_ProfileService>();
            return services;
        }
    }
}