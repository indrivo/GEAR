using GR.Audit.Abstractions.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Identity.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add context and identity
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityModule<TContext>(this IServiceCollection services)
            where TContext : DbContext, IIdentityContext
        {
            services.AddIdentity<GearUser, GearRole>()
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
            where TUser : GearUser
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
            services.AddDbContext<TIdentityContext>(builder
                => builder.RegisterIdentityStorage(configuration, migrationsAssembly));

            services.RegisterAuditFor<IIdentityContext>("Identity module");
            //SystemEvents.Database.OnMigrate += (sender, args) =>
            //{
            //    GearApplication.GetHost<IWebHost>().MigrateDbContext<TIdentityContext>();
            //};
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

        /// <summary>
        /// Register location service
        /// </summary>
        /// <typeparam name="TLocationService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterLocationService<TLocationService>(this IServiceCollection services)
            where TLocationService : class, ILocationService
        {
            services.AddSingleton<ILocationService, TLocationService>();
            return services;
        }
    }
}