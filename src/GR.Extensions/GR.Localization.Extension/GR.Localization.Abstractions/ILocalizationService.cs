using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.Abstractions
{
	public interface ILocalizationService
	{
		/// <summary>
		/// Edit translation key
		/// </summary>
		/// <param name="model"></param>
		void EditKey(EditLocalizationViewModel model);

		/// <summary>
		/// Add or update key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="localizedStrings"></param>
		void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings);

		/// <summary>
		/// Add new language
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		ResultModel AddLanguage(AddLanguageViewModel model);

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
    }
}