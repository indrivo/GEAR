using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Localization.Abstractions.ViewModels.LocalizationViewModels
{
	public class AddKeyViewModel
	{
		[Required]
		public string NewKey { get; set; }

		public Dictionary<string, string> LocalizedStrings { get; set; }
		public Dictionary<string, string> Languages { get; set; }
	}
}