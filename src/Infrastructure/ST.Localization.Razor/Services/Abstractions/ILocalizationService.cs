using System.Collections.Generic;
using ST.Core.Helpers;
using ST.Localization.Razor.ViewModels.LocalizationViewModels;

namespace ST.Localization.Razor.Services.Abstractions
{
	public interface ILocalizationService
	{
		void EditKey(EditLocalizationViewModel model);
		void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings);
		ResultModel AddLanguage(AddLanguageViewModel model);
		ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model);
	}
}