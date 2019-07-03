using System;
using System.ComponentModel.DataAnnotations;
using ST.Identity.Abstractions.Enums;

namespace ST.Identity.Razor.Users.ViewModels.UserViewModels
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
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
             MinimumLength = 6), DataType(DataType.Password)]
        [Required]
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

        public  string CallBackUrl { get; set; }
    }
}
