using System;
using System.Globalization;
using System.Linq;
using GR.Audit.Abstractions.Extensions;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using GR.Core.Helpers;
using GR.Localization.Abstractions.Events;
using GR.Localization.Abstractions.Models.Config;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace GR.Localization.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add localization
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationModule<TService, TStringLocalizer>(this IServiceCollection services)
            where TService : class, ILocalizationService
            where TStringLocalizer : class, IStringLocalizer
        {
            Arg.NotNull(services, nameof(AddLocalizationModule));

            services.AddTransient<ILocalizationService, TService>();
            services.AddTransient<IStringLocalizer, TStringLocalizer>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromDays(1);
                opts.Cookie.HttpOnly = true;
            });

            LocalizationEvents.RegisterEvents();

            //TODO: Translate form validations
            //services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();
            //services.AddMvc()
            //    .AddDataAnnotationsLocalization(o =>
            //        {
            //            o.DataAnnotationLocalizerProvider = (type, factory) => services.BuildServiceProvider().GetRequiredService<IStringLocalizer>();
            //        });
            return services;
        }

        /// <summary>
        /// Register translation service
        /// </summary>
        /// <typeparam name="TExternalTranslationProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterTranslationService<TExternalTranslationProvider>(this IServiceCollection services, IConfigurationSection section)
            where TExternalTranslationProvider : class, IExternalTranslationProvider
        {
            services.AddTransient<IExternalTranslationProvider, TExternalTranslationProvider>();
            services.Configure<LocalizationProviderSettings>(section);
            return services;
        }


        /// <summary>
        /// Add country service
        /// </summary>
        /// <typeparam name="TCountryService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCountryModule<TCountryService>(this IServiceCollection services)
            where TCountryService : class, ICountryService
        {
            services.AddGearTransient<ICountryService, TCountryService>();
            return services;
        }

        /// <summary>
        /// Add country module storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddCountryModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, ICountryContext
        {
            services.AddTransient<ICountryContext, TContext>();
            services.AddDbContext<TContext>(options);
            services.RegisterAuditFor<TContext>("Countries module");
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }

        /// <summary>
        /// Add localization module
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationModuleStorage<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, ILocalizationContext
        {
            services.AddGearScoped<ILocalizationContext, TContext>();
            services.AddDbContext<TContext>(options);
            services.RegisterAuditFor<TContext>("Localization module");
            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost<IWebHost>().MigrateDbContext<TContext>();
            };
            return services;
        }


        /// <summary>
        /// Use Localization
        /// </summary>
        /// <param name="app"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLocalizationModule(this IApplicationBuilder app,
            IOptionsSnapshot<LocalizationConfig> language)
        {
            var supportedCultures = language.Value.Languages.Select(str => new CultureInfo(str.Identifier)).ToList();
            var opts = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(opts);
            var locMon = app.ApplicationServices.GetRequiredService<IOptionsMonitor<LocalizationConfigModel>>();
            locMon.OnChange(locConfig =>
            {
                var languages = locConfig.Languages.Select(lStr => new CultureInfo(lStr.Identifier)).ToList();
                var reqLoc = app.ApplicationServices.GetRequiredService<IOptionsSnapshot<RequestLocalizationOptions>>();

                reqLoc.Value.SupportedCultures = languages;
                reqLoc.Value.SupportedUICultures = languages;
            });
            return app;
        }
    }
}
