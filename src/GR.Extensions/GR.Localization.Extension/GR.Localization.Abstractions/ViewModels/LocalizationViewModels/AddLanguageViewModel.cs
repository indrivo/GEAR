using System.ComponentModel.DataAnnotations;

namespace GR.Localization.Abstractions.ViewModels.LocalizationViewModels
{
	public class AddLanguageViewModel
	{
		[Required]
		[StringLength(2)]
		public string Identifier { get; set; }

		[Required]
		public string Name { get; set; }
	}
}