using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.Abstractions
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Edit translation key
        /// </summary>
        /// <param name="model"></param>
        Task<ResultModel> EditKeyAsync(EditLocalizationViewModel model);

        /// <summary>
        /// Add or update key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="localizedStrings"></param>
        Task<ResultModel> AddOrUpdateKeyAsync(string key, IDictionary<string, string> localizedStrings);

        /// <summary>
        /// Add new language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddLanguageAsync(AddLanguageViewModel model);

        /// <summary>
        /// Enable or disable language
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model);

        /// <summary>
        /// Import language translations
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        Task<ResultModel> ImportLanguageTranslationsAsync(string identifier, Dictionary<string, string> translations);

        /// <summary>
        /// Get all languages
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<LanguageCreateViewModel>>> GetAllLanguagesAsync();

        /// <summary>
        /// Get language by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        Task<ResultModel<Language>> GetLanguageByIdentifierAsync(string identifier);

        /// <summary>
        /// Generate key
        /// </summary>
        /// <returns></returns>
        string GenerateKey(string language, string key);

        /// <summary>
        /// Get language pack
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, string>>> GetLanguagePackAsync(string language);

        /// <summary>
        /// Get language packs
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, Dictionary<string, string>>>> GetLanguagePacksAsync();
    }
}