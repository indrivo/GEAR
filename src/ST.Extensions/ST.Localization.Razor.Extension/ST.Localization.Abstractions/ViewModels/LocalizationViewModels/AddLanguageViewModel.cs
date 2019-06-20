using System.ComponentModel.DataAnnotations;

namespace ST.Localization.Abstractions.ViewModels.LocalizationViewModels
{
	public class AddLanguageViewModel
	{
		[Required]
		public string Identifier { get; set; }

		[Required]
		public string Name { get; set; }
	}
}