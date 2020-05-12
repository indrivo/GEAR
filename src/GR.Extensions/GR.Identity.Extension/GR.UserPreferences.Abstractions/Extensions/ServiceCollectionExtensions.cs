using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.UserPreferences.Abstractions.Helpers;
using GR.UserPreferences.Abstractions.Helpers.PreferenceTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.UserPreferences.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register user preferences module
        /// </summary>
        /// <typeparam name="TUserPreferencesService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserPreferencesModule<TUserPreferencesService>(this IServiceCollection services)
            where TUserPreferencesService : class, IUserPreferencesService
        {
            services.AddGearScoped<IUserPreferencesService, TUserPreferencesService>();
            return services;
        }

        /// <summary>
        /// Add user preferences storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserPreferencesModuleStorage<TContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IUserPreferencesContext
        {
            services.AddDbContext<TContext>(options);
            services.AddScopedContextFactory<IUserPreferencesContext, TContext>();

            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };

            return services;
        }

        /// <summary>
        /// Register preferences provider
        /// </summary>
        /// <typeparam name="TPreferencesProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPreferencesProvider<TPreferencesProvider>(this IServiceCollection services)
            where TPreferencesProvider : DefaultUserPreferenceProvider
        {
            IoC.RegisterSingletonService<DefaultUserPreferenceProvider>("PreferencesProvider_Instance", typeof(TPreferencesProvider));

            //Register timeZone
            services.RegisterUserPreferenceKey<ListPreferenceItem>(UserPreferencesResources.UserTimeZoneKey, options =>
            {
                options.IsRequired = true;
                options.IsValidValue = value =>
                {
                    var response = new ResultModel();
                    if (value.IsNullOrEmpty()) return response;
                    try
                    {
                        TimeZoneInfo.FindSystemTimeZoneById(value);
                        response.IsSuccess = true;
                        return response;
                    }
                    catch (Exception e)
                    {
                        response.AddError(e.Message);
                    }

                    return response;
                };

                options.ResolveListItems = selectedZone =>
                {
                    //$"({zone.StandardName}) {zone.Id}"
                    var data = TimeZoneInfo.GetSystemTimeZones()
                        .Select(zone =>
                            new DisplayItem
                            {
                                Id = zone.Id,
                                Label = zone.DisplayName,
                                Selected = selectedZone == zone.Id
                            }).ToList();

                    return Task.FromResult<IEnumerable<DisplayItem>>(data);
                };
                return options;
            });

            return services;
        }

        /// <summary>
        /// Register user preferences key
        /// </summary>
        /// <typeparam name="TPreferenceItem"></typeparam>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterUserPreferenceKey<TPreferenceItem>(this IServiceCollection services, string key, Func<TPreferenceItem, TPreferenceItem> options)
            where TPreferenceItem : PreferenceItem, new()
        {
            var provider = IoC.Resolve<DefaultUserPreferenceProvider>("PreferencesProvider_Instance");
            provider.RegisterPreferenceItem(key, options(new TPreferenceItem
            {
                Key = key
            }));
            return services;
        }
    }
}