using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using ST.Localization.Razor.Services;
using ST.Localization.Razor.Services.Abstractions;
using ST.Localization.Razor.ViewModels.LocalizationViewModels;

namespace ST.Localization.Razor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Use Localization
        /// </summary>
        /// <param name="app"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app,
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
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalization(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddTransient<IStringLocalizer, JsonStringLocalizer>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.Configure<LocalizationConfigModel>(configuration.GetSection(nameof(LocalizationConfig)));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromDays(1);
                opts.Cookie.HttpOnly = true;
            });
            return services;
        }
    }
}
