using System;
using GR.AccountActivity.Abstractions.ActionFilters;
using GR.AccountActivity.Abstractions.Helpers;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Identity.Abstractions.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.AccountActivity.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add user activity module
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TActivityFilter"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserActivityModule<TService, TActivityFilter>(this IServiceCollection services)
            where TService : class, IUserActivityService
            where TActivityFilter : ActivityTrackerFilter
        {
            services.AddGearScoped<IUserActivityService, TService>();
            services.AddDetection();

            services.AddMvc(opt =>
            {
                opt.Filters.Add<TActivityFilter>();
            });

            IdentityEvents.Authorization.OnUserLogIn += (sender, args) =>
            {
                var service = args.InjectService<IUserActivityService>();
                service.RegisterUserNotAuthenticatedActivityAsync(args.UserId, AccountActivityResources.ActivityTypes.SIGNIN, args.HttpContext);
            };

            IdentityEvents.Authorization.OnUserLogout += (sender, args) =>
            {
                var service = args.InjectService<IUserActivityService>();
                service.RegisterUserActivityAsync(AccountActivityResources.ActivityTypes.SIGNOUT, args.HttpContext);
            };

            return services;
        }

        /// <summary>
        /// Add user activity module storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserActivityModuleStorage<TContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IActivityContext
        {
            services.AddDbContext<TContext>(options);
            services.AddScopedContextFactory<IActivityContext, TContext>();

            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };

            return services;
        }

        /// <summary>
        /// Register token provider
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityBuilder AddConfirmDeviceTokenProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var provider = typeof(DataProtectorTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider(AccountActivityResources.TrackActivityTokenProvider, provider);
        }
    }
}
