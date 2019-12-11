using System.Collections.Generic;
using GR.Core.Helpers;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.Abstractions
{
	public interface ILocalizationService
	{
		void EditKey(EditLocalizationViewModel model);
		void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings);
		ResultModel AddLanguage(AddLanguageViewModel model);
		ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model);
	}
}