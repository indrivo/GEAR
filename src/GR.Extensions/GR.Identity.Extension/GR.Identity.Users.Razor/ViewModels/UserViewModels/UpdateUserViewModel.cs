using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Enums;
using GR.Identity.Abstractions.Models.MultiTenants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Users.Razor.ViewModels.UserViewModels
{
    public class UpdateUserViewModel : BaseModel
    {
        public UpdateUserViewModel()
        {
            Roles = new HashSet<GearRole>();
            Tenants = new HashSet<Tenant>();
            SelectedRoleId = new HashSet<string>();
        }

        [Display(Name = "First Name", Prompt = "first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name", Prompt = "last name")]
        public string LastName { get; set; }

        public IEnumerable<GearRole> Roles { get; set; }
        public IEnumerable<Tenant> Tenants { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Select a role for this user"),
         Display(Name = "User's Role")]
        public IEnumerable<string> SelectedRoleId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User Name")]
        [Remote(action: "VerifyName", controller: "Users", AdditionalFields =
            nameof(UserName) + "," + nameof(UserNameOld))]
        public string UserName { get; set; }

        public string UserNameOld { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
             MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords should match")]
        [Display(Name = "Repeat Password")]
        public string RepeatPassword { get; set; }

        [Required, EmailAddress] public string Email { get; set; }

        public string ProfilesJson { get; set; }
        [Display(Name = "User Photo")] public byte[] UserPhoto { get; set; }
        [Display(Name = "User Photo")] public IFormFile UserPhotoUpdateFile { get; set; }

        /// <summary>
        /// Authentication Type
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.Local;
    }
}