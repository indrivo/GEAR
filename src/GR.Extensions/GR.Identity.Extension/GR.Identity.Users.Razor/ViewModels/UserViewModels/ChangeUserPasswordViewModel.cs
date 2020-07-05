using System;
using System.ComponentModel.DataAnnotations;
using GR.Identity.Abstractions.Helpers;

namespace GR.Identity.Users.Razor.ViewModels.UserViewModels
{
    public sealed class ChangeUserPasswordViewModel
    {
        /// <summary>
        /// User id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = IdentityResources.ValidationMessages.PASSWORD_STRING_LENGTH, MinimumLength = 6), DataType(DataType.Password)]
        [RegularExpression(IdentityResources.RegularExpressions.PASSWORD, ErrorMessage = IdentityResources.ValidationMessages.PASSWORD_COMPLEXITY_MESSAGE)]
        public string Password { get; set; }

        /// <summary>
        /// Repeat new password
        /// </summary>
        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords should match")]
        [Display(Name = "Repeat Password")]
        [Required]
        public string RepeatPassword { get; set; }

        /// <summary>
        /// Authentication type
        /// </summary>
        public string AuthenticationType { get; set; }

        public string CallBackUrl { get; set; }
    }
}