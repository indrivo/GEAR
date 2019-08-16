using System.ComponentModel.DataAnnotations;

namespace ST.Identity.Razor.ViewModels.AccountViewModels
{
    public class ConfirmEmailViewModel
    {
        public string UserId { get; set; }
        [Display(Name = "User Name")] public string UserName { get; set; }
        [Display(Name = "Email")] public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
             MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Repeat Password")]
        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords should match")]
        public string RepeatPassword { get; set; }

        public string Token { get; set; }
    }
}