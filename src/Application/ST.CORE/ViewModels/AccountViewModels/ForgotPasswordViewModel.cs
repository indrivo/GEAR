using System.ComponentModel.DataAnnotations;

namespace ST.CORE.ViewModels.AccountViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}