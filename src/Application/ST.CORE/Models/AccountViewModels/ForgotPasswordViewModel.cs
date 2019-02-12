using System.ComponentModel.DataAnnotations;

namespace ST.CORE.Models.AccountViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}