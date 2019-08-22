using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Enums;
using ST.Identity.Abstractions.Models.AddressModels;
using ST.Identity.Abstractions.Models.MultiTenants;

namespace ST.Identity.Razor.Users.ViewModels.UserViewModels
{
    public class CreateUserViewModel
    {
        public CreateUserViewModel()
        {
            Tenants = new HashSet<Tenant>();
            Roles = new HashSet<ApplicationRole>();
            Groups = new HashSet<AuthGroup>();
            SelectedRoleId = new HashSet<string>();
            SelectedGroupId = new HashSet<string>();
            Profiles = new HashSet<EntityViewModel>();
            CountrySelectListItems = new HashSet<SelectListItem>();
            Address = new Address();
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
        public IEnumerable<Tenant> Tenants { get; set; }

        public IEnumerable<ApplicationRole> Roles { get; set; }
        public IEnumerable<AuthGroup> Groups { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Select a role for this user"),
         Display(Name = "User's Role")]
        public IEnumerable<string> SelectedRoleId { get; set; }

        [Display(Name = "User's group")] public IEnumerable<string> SelectedGroupId { get; set; }

        public IEnumerable<EntityViewModel> Profiles { get; set; }

        [Display(Name = "Select country")] public string SelectedCountryId { get; set; }
        public IEnumerable<SelectListItem> CountrySelectListItems { get; set; }

        [Display(Name = "Select city")] public int? SelectedCityId { get; set; }

        public Address Address { get; set; }

        /// <summary>
        /// User organization
        /// </summary>
        [Display(Name = "Select user organization")]
        public Guid? TenantId { get; set; }
    }
}