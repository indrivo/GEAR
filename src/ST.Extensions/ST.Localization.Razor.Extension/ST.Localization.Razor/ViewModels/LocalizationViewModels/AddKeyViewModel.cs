using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST.Localization.Razor.ViewModels.LocalizationViewModels
{
	public class AddKeyViewModel
	{
		[Required]
		public string NewKey { get; set; }

		public Dictionary<string, string> LocalizedStrings { get; set; }
		public Dictionary<string, string> Languages { get; set; }
	}
}