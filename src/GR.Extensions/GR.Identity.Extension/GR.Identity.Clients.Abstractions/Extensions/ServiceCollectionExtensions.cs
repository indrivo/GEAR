using System;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Configurations;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Clients.Abstractions.Helpers;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
        /// <param name="configuration"></param>
        /// <param name="migrationsAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityClientsModule<TUser, TConfiguration, TPersisted>(this IServiceCollection services, IConfiguration configuration, string migrationsAssembly)
            where TUser : GearUser
            where TConfiguration : ConfigurationDbContext<TConfiguration>, IClientsContext
            where TPersisted : PersistedGrantDbContext<TPersisted>, IClientsPersistedGrantContext
        {
            Action<DbContextOptionsBuilder> dbOptions = builder => builder.RegisterIdentityStorage(configuration, migrationsAssembly);
            services.AddIdentityServer(x =>
                {
                    x.IssuerUri = "null";
                    x.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                })
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<TUser>()
                .AddConfigurationStore<TConfiguration>(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = dbOptions;
                })
                .AddOperationalStore<TPersisted>(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = dbOptions;
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

            services.AddGearSingleton<IClientsContext, TConfiguration>();
            services.AddGearSingleton<IClientsPersistedGrantContext, TPersisted>();

            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>()
                    .MigrateDbContext<TPersisted>()
                    .MigrateDbContext<TConfiguration>((context, servicesProvider) =>
                    {
                        var configurator = new DefaultClientsConfigurator();
                        ClientsSeeder.SeedAsync(servicesProvider, configurator)
                            .Wait();
                    });
            };

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
            services.AddTransient<IClientsService, TService>();
            return services;
        }
    }
}