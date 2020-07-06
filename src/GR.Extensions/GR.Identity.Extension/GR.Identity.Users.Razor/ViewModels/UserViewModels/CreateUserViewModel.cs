using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.Identity.Abstractions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Identity.Users.Razor.ViewModels.UserViewModels
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class CreateUserViewModel
    {
        public CreateUserViewModel()
        {
            Tenants = new HashSet<SelectListItem>();
            Roles = new HashSet<SelectListItem>();
            SelectedRoleId = new HashSet<Guid>();
        }

        [Display(Name = "First Name", Prompt = "first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name", Prompt = "last name")]
        public string LastName { get; set; }

        [MaxLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [MaxLength(500)] public string AboutMe { get; set; }
        public DateTime? Birthday { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [Remote("VerifyName", "Users")]
        public string UserName { get; set; }

        [Display(Name = "Is Deleted")] public bool IsDeleted { get; set; }

        [Required,
         StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
             MinimumLength = 6), DataType(DataType.Password)]
        [RegularExpression(IdentityResources.RegularExpressions.PASSWORD, ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords should match")]
        [Display(Name = "Repeat Password")]
        public string RepeatPassword { get; set; }

        [Required, EmailAddress] public string Email { get; set; }

        /// <summary>
        /// Authentication Type
        /// </summary>
        [Required]
        [Display(Name = "Select Authentication Type")]
        public string AuthenticationType { get; set; }

        public string ProfilesJson { get; set; }

        [Display(Name = "User Photo")] public IFormFile UserPhoto { get; set; }

        /// <summary>
        /// List with tenants
        /// </summary>
        public IEnumerable<SelectListItem> Tenants { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Select a role for this user"),
         Display(Name = "User's Role")]
        public IEnumerable<Guid> SelectedRoleId { get; set; }

        /// <summary>
        /// User organization
        /// </summary>
        [Display(Name = "Select user organization")]
        public Guid? TenantId { get; set; }
    }
}