using System.Collections.Generic;

namespace GR.Localization.Abstractions.ViewModels.LocalizationViewModels
{
	public class LocalizationConfigModel
	{
		public HashSet<LanguageCreateViewModel> Languages { get; set; }
		public string Path { get; set; }
		public string SessionStoreKeyName { get; set; }
		public string DefaultLanguage { get; set; }
	}
}
