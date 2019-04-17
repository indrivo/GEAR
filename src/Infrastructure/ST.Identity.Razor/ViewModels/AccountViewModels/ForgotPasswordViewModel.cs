using System.ComponentModel.DataAnnotations;

namespace ST.Identity.Razor.ViewModels.AccountViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}