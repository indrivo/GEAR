using System;
using System.ComponentModel.DataAnnotations;
using GR.Identity.Abstractions.Enums;
using GR.Identity.Abstractions.Helpers;

namespace GR.Identity.Razor.Users.ViewModels.UserViewModels
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
        [StringLength(100, ErrorMessage = Resources.ValidationMessages.PASSWORD_STRING_LENGTH, MinimumLength = 6), DataType(DataType.Password)]
        [RegularExpression(Resources.RegularExpressions.PASSWORD, ErrorMessage = Resources.ValidationMessages.PASSWORD_COMPLEXITY_MESSAGE)]
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
        public AuthenticationType AuthenticationType { get; set; }

        public string CallBackUrl { get; set; }
    }
}
