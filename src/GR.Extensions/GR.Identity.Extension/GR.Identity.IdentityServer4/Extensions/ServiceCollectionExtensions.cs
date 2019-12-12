using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Configurations;
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
        /// <param name="hostingEnvironment"></param>
        /// <param name="migrationsAssembly"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment hostingEnvironment,
            string migrationsAssembly)
        {
            services.AddIdentityServer(x => x.IssuerUri = "null")
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<GearUser>()
                .AddConfigurationStore(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = builder =>
                    {
                        var connectionString = DbUtil.GetConnectionString(configuration);
                        if (connectionString.Item1 == DbProviderType.PostgreSql)
                        {
                            builder.UseNpgsql(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    IdentityConfig.DEFAULT_SCHEMA);
                            });
                        }
                        else
                        {
                            builder.UseSqlServer(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    IdentityConfig.DEFAULT_SCHEMA);
                            });
                        }
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = builder =>
                    {
                        var connectionString = DbUtil.GetConnectionString(configuration);
                        if (connectionString.Item1 == DbProviderType.PostgreSql)
                        {
                            builder.UseNpgsql(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    IdentityConfig.DEFAULT_SCHEMA);
                            });
                        }
                        else
                        {
                            builder.UseSqlServer(connectionString.Item2, opts =>
                            {
                                opts.MigrationsAssembly(migrationsAssembly);
                                opts.MigrationsHistoryTable("IdentityServerConfigurationMigrationHistory",
                                    IdentityConfig.DEFAULT_SCHEMA);
                            });
                        }
                    };
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
