using System.Collections.Generic;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Helpers;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Localization.Extensions
{
    public static class ServiceCollectionsExtensions
    {
        /// <summary>
        /// Bind languages
        /// </summary>
        /// <param name="services"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection BindLanguagesFromJsonFile(this IServiceCollection services, IConfigurationSection section)
        {
            services.Configure<LocalizationConfigModel>(section);
            return services;
        }

        /// <summary>
        /// Bind languages from database
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection BindLanguagesFromDatabase(this IServiceCollection services)
        {
            services.Configure<LocalizationConfigModel>(conf =>
            {
                var service = IoC.Resolve<ILocalizationService>();
                var languagesRequest =  service.GetAllLanguagesAsync().ExecuteAsync();
                conf.DefaultLanguage = LocalizationResources.DEFAULT_LANGUAGE_IDENTIFIER;
                if (languagesRequest.IsSuccess)
                {
                    conf.Languages = new HashSet<LanguageCreateViewModel>(languagesRequest.Result ?? new List<LanguageCreateViewModel>());
                }
                else conf.Languages = new HashSet<LanguageCreateViewModel>
                {
                    new LanguageCreateViewModel
                    {
                        Name = GearSettings.DEFAULT_LANGUAGE,
                        Identifier = LocalizationResources.DEFAULT_LANGUAGE_IDENTIFIER
                    }
                };
                conf.SessionStoreKeyName = "lang";
            });
            return services;
        }
    }
}
