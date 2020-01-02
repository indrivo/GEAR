using System;
using Castle.MicroKernel.Registration;
using GR.Audit.Abstractions.Extensions;
using GR.Core.Extensions;
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
            services.AddIdentity<GearUser, GearRole>(options =>
                {
                    options.Lockout = new LockoutOptions
                    {
                        MaxFailedAccessAttempts = 4,
                        AllowedForNewUsers = true,
                        DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15)
                    };

                    //options.SignIn = new SignInOptions
                    //{
                    //    RequireConfirmedEmail = true
                    //};
                })
                .AddEntityFrameworkStores<TContext>()
                .AddDefaultTokenProviders();

            //Register user manager
            IoC.Container.Register(Component.For<UserManager<GearUser>>());
            return services;
        }


        /// <summary>
        /// Add authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authority = configuration.GetSection("WebClients").GetSection("CORE");
            var uri = authority.GetValue<string>("uri");

            services.AddAuthentication()
                .AddJwtBearer(opts =>
                {
                    opts.Audience = "core";
                    opts.Authority = uri;
                    opts.RequireHttpsMetadata = false;
                });
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
            services.AddGearTransient<IUserManager<TUser>, TUserManager>();
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
            services.AddScopedContextFactory<IIdentityContext, TIdentityContext>();
            services.AddDbContext<TIdentityContext>(builder
                => builder.RegisterIdentityStorage(configuration, migrationsAssembly), ServiceLifetime.Transient);

            services.RegisterAuditFor<IIdentityContext>("Identity module");
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
            services.AddGearTransient<IAppProvider, TAppProvider>();
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
            services.AddGearTransient<IUserAddressService, TAddressService>();
            return services;
        }

        /// <summary>
        /// Register group repository
        /// </summary>
        /// <typeparam name="TGroupRepository"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterGroupRepository<TGroupRepository, TContext, TUser>(this IServiceCollection services)
            where TGroupRepository : class, IGroupRepository<TContext, TUser>
            where TContext : DbContext where TUser : IdentityUser
        {
            services.AddGearTransient<IGroupRepository<TContext, TUser>, TGroupRepository>();
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
            services.AddGearSingleton<ILocationService, TLocationService>();
            return services;
        }
    }
}