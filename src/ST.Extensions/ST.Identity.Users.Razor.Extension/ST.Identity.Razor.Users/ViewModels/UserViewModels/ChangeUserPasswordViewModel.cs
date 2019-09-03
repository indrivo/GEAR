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
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
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
