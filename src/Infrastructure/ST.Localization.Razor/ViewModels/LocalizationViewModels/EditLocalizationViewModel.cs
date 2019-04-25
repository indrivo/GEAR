using System.Collections.Generic;

namespace ST.Localization.Razor.ViewModels.LocalizationViewModels
{
	public class EditLocalizationViewModel
	{
		public string Key { get; set; }
		public Dictionary<string, string> LocalizedStrings { get; set; }
		public Dictionary<string, string> Languages { get; set; }
	}
}