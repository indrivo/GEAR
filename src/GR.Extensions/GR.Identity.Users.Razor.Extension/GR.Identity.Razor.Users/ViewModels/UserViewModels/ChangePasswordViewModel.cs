using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Razor.Users.ViewModels.UserViewModels
{
    public sealed class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
             MinimumLength = 6), DataType(DataType.Password)]
        [Required]
        [Display(Name = "New password")]
        public string Password { get; set; }

        /// <summary>
        /// Repeat new password
        /// </summary>
        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords should match")]
        [Display(Name = "Repeat Password")]
        [Required]
        public string RepeatPassword { get; set; }
    }
}