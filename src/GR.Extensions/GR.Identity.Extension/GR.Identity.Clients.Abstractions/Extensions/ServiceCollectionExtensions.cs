using System;
using System.Linq;
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
        /// <returns></returns>
        public static IServiceCollection AddIdentityClientsModule<TUser, TConfiguration, TPersisted, TClientsConfiguration>(this IServiceCollection services, IConfiguration configuration)
            where TClientsConfiguration : DefaultClientsConfigurator
            where TUser : GearUser
            where TConfiguration : ConfigurationDbContext<TConfiguration>, IClientsContext
            where TPersisted : PersistedGrantDbContext<TPersisted>, IClientsPersistedGrantContext
        {
            var configurator = Activator.CreateInstance<TClientsConfiguration>();
            var migrationsAssembly = typeof(TConfiguration).Assembly.GetName().Name;

            void DbOptions(DbContextOptionsBuilder builder) => builder.RegisterIdentityStorage(configuration, migrationsAssembly);
            var clientsSection = configuration.GetSection(ClientResources.WebClientsSection)
                .GetChildren()
                .Select(x => x.Key)
                .ToList();

            var clientUrls = clientsSection
                .ToDictionary(sectionClient => sectionClient,
                    sectionClient => ClientsSeeder.GetClientUrl(configuration, sectionClient));

            services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<TUser>()
                .AddConfigurationStore<TConfiguration>(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = DbOptions;
                })
                .AddOperationalStore<TPersisted>(options =>
                {
                    options.DefaultSchema = IdentityConfig.DEFAULT_SCHEMA;
                    options.ConfigureDbContext = DbOptions;
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddInMemoryIdentityResources(configurator.GetResources())
                .AddInMemoryApiResources(configurator.GetApiResources())
                .AddInMemoryClients(configurator.GetClients(clientUrls));
                //.AddConfigurationStoreCache();
                //.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

            services.AddGearSingleton<IClientsContext, TConfiguration>();
            services.AddGearSingleton<IClientsPersistedGrantContext, TPersisted>();

            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>()
                    .MigrateDbContext<TPersisted>()
                    .MigrateDbContext<TConfiguration>((context, servicesProvider) =>
                    {
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