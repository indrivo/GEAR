using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Configurations;
using GR.Identity.Abstractions.Events;

namespace GR.Identity.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add context and identity
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="migrationsAssembly"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityModule<TContext>(this IServiceCollection services,
            IConfiguration configuration, IHostingEnvironment hostingEnvironment, string migrationsAssembly,
            IHostingEnvironment environment)
            where TContext : DbContext, IIdentityContext
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();
            return services;
        }

        /// <summary>
        /// Add identity user manager
        /// </summary>
        /// <typeparam name="TUserManager"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityUserManager<TUserManager, TUser>(this IServiceCollection services)
            where TUser : ApplicationUser
            where TUserManager : class, IUserManager<TUser>
        {
            services.AddTransient<IUserManager<TUser>, TUserManager>();
            return services;
        }

        /// <summary>
        /// Add identity storage
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="migrationsAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityModuleStorage<TIdentityContext>(this IServiceCollection services,
            IConfiguration configuration, string migrationsAssembly)
            where TIdentityContext : DbContext, IIdentityContext
        {
            services.AddTransient<IIdentityContext, TIdentityContext>();
            services.AddDbContext<TIdentityContext>(options =>
            {
                var connectionString = DbUtil.GetConnectionString(configuration);
                if (connectionString.Item1 == DbProviderType.PostgreSql)
                {
                    options.UseNpgsql(connectionString.Item2, opts =>
                    {
                        opts.MigrationsAssembly(migrationsAssembly);
                        opts.MigrationsHistoryTable("IdentityMigrationHistory", IdentityConfig.DEFAULT_SCHEMA);
                    });
                }
                else
                {
                    options.UseSqlServer(connectionString.Item2, opts =>
                    {
                        opts.MigrationsAssembly(migrationsAssembly);
                        opts.MigrationsHistoryTable("IdentityMigrationHistory", IdentityConfig.DEFAULT_SCHEMA);
                    });
                }
            });

            services.RegisterAuditFor<IIdentityContext>("Identity module");
            SystemEvents.Database.OnMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TIdentityContext>();
            };
            return services;
        }

        /// <summary>
        /// Add provider
        /// </summary>
        /// <typeparam name="TAppProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppProvider<TAppProvider>(this IServiceCollection services)
            where TAppProvider : class, IAppProvider
        {
            services.AddTransient<IAppProvider, TAppProvider>();
            IoC.RegisterTransientService<IAppProvider, TAppProvider>();
            return services;
        }

        /// <summary>
        /// Register address service
        /// </summary>
        /// <typeparam name="TAddressService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserAddressService<TAddressService>(this IServiceCollection services)
            where TAddressService : class, IUserAddressService
        {
            services.AddTransient<IUserAddressService, TAddressService>();
            IoC.RegisterTransientService<IUserAddressService, TAddressService>();
            return services;
        }

        /// <summary>
        /// Register events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityModuleEvents(this IServiceCollection services)
        {
            IdentityEvents.RegisterEvents();
            return services;
        }
    }
}