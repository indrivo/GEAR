using System.Collections.Generic;
using ST.BaseBusinessRepository;
using ST.CORE.Models.LocalizationViewModels;

namespace ST.CORE.Services.Abstraction
{
	public interface ILocalizationService
	{
		void EditKey(EditLocalizationViewModel model);
		void AddOrUpdateKey(string key, IDictionary<string, string> localizedStrings);
		ResultModel AddLanguage(AddLanguageViewModel model);
		ResultModel ChangeStatusOfLanguage(LanguageCreateViewModel model);
	}
}