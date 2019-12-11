using System.ComponentModel.DataAnnotations;
using GR.Identity.Abstractions.Helpers;

namespace GR.MultiTenant.Razor.ViewModels
{
    public class ConfirmEmailViewModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        [Display(Name = "User Name")] public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")] public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Display(Name = "Password")]
        [Required]
        [StringLength(100, ErrorMessage = Resources.ValidationMessages.PASSWORD_STRING_LENGTH, MinimumLength = 6), DataType(DataType.Password)]
        [RegularExpression(Resources.RegularExpressions.PASSWORD, ErrorMessage = Resources.ValidationMessages.PASSWORD_COMPLEXITY_MESSAGE)]
        public string Password { get; set; }

        /// <summary>
        /// Repeat password
        /// </summary>
        [Required]
        [Display(Name = "Repeat Password")]
        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords should match")]
        public string RepeatPassword { get; set; }

        /// <summary>
        /// Email token
        /// </summary>
        public string Token { get; set; }
    }
}