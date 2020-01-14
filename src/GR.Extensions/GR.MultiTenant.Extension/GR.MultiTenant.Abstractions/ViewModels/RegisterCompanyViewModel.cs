using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes;
using GR.Identity.Abstractions.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GR.MultiTenant.Abstractions.ViewModels
{
    public class RegisterCompanyViewModel : CreateTenantViewModel
    {
        /// <summary>
        /// First name
        /// </summary>
       // [Required]
        [MaxLength(50)]
        [DisplayTranslate(Key = "system_first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
       // [Required]
        [MaxLength(50)]
        [DisplayTranslate(Key = "system_last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        //[Required]
        [MaxLength(30)]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        [DisplayTranslate(Key = "system_auth_username")]
        [Remote("CheckUserNameIfExist", "CompanyManage")]
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required, DataType(DataType.EmailAddress)]
        [DisplayTranslate(Key = "email")]
        [EmailAddress]
        [Remote("CheckEmailIfExist", "CompanyManage")]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required,
         StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
             MinimumLength = 6), DataType(DataType.Password)]
        [DisplayTranslate(Key = "system_auth_password")]
        [RegularExpression(Resources.RegularExpressions.PASSWORD, ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        /// <summary>
        /// Repeat password
        /// </summary>
        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords should match")]
        [DisplayTranslate(Key = "system_auth_repeat_password")]
        [Display(Name = "Repeat Password")]
        public string RepeatPassword { get; set; }
    }
}
