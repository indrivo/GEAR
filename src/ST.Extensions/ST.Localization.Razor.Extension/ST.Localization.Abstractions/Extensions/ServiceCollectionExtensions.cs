using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ST.Core.Helpers;
using ST.Localization.Abstractions.Models;
using ST.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace ST.Localization.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
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

        /// <summary>
        /// Add localization
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationModule<TService, TExternalTranslationProvider>(this IServiceCollection services, TranslationModuleOptions options)
            where TService : class, ILocalizationService
            where TExternalTranslationProvider : class, IExternalTranslationProvider
        {
            Arg.NotNull(services, nameof(AddLocalizationModule));
            Arg.NotNull(options, nameof(TranslationModuleOptions));
            Arg.NotNull(options.Configuration, nameof(TranslationModuleOptions.Configuration));
            services.AddTransient<ILocalizationService, TService>();
            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddTransient<IExternalTranslationProvider, TExternalTranslationProvider>();
            services.Configure<LocalizationConfigModel>(options.Configuration.GetSection(nameof(LocalizationConfig)));
            services.Configure<LocalizationProviderSettings>(opts =>
                options.Configuration.GetSection(nameof(LocalizationProviderSettings)).Bind(opts));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromDays(1);
                opts.Cookie.HttpOnly = true;
            });
            //TODO: ?
            switch (options.LocalizationProvider)
            {
                case LocalizationProvider.Google:

                    break;
                case LocalizationProvider.Yandex:
                    break;
            }
            return services;
        }
    }
}
