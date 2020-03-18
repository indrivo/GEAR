using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes.Documentation;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Identity.Abstractions.Enums;
using GR.Identity.Abstractions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Identity.Users.Razor.ViewModels.UserViewModels
{
    [Author("Lupei Nicolae", 1.1)]
    [Author("Nirca Cristian", 1.2, "Add country and city, address")]
    [Author("Lupei Nicolae", 1.3, "Fix non used address field, what cause exception on user create")]
    public class CreateUserViewModel
    {
        public CreateUserViewModel()
        {
            Tenants = new HashSet<SelectListItem>();
            Roles = new HashSet<SelectListItem>();
            SelectedRoleId = new HashSet<Guid>();
            Profiles = new HashSet<EntityViewModel>();
            CountrySelectListItems = new HashSet<SelectListItem>();
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
        [RegularExpression(Resources.RegularExpressions.PASSWORD, ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
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
        public AuthenticationType AuthenticationType { get; set; }

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

        public IEnumerable<EntityViewModel> Profiles { get; set; }

        [Display(Name = "Select country")] public string SelectedCountryId { get; set; }
        public IEnumerable<SelectListItem> CountrySelectListItems { get; set; }

        [Display(Name = "Select city")] public int? SelectedCityId { get; set; }

        /// <summary>
        /// User organization
        /// </summary>
        [Display(Name = "Select user organization")]
        public Guid? TenantId { get; set; }
    }
}