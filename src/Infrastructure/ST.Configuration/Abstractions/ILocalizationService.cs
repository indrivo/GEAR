using System.Collections.Generic;
using ST.BaseBusinessRepository;
using ST.Configuration.ViewModels.LocalizationViewModels;

namespace ST.Configuration.Abstractions
{
	public interface ILocalizationService
	{
		void EditKey(EditLocalizationViewModel model);
		void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings);
		ResultModel AddLanguage(AddLanguageViewModel model);
		ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model);
	}
}