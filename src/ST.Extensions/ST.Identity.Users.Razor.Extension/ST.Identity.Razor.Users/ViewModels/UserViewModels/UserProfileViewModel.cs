using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Identity.Abstractions.Models.MultiTenants;

namespace ST.Identity.Razor.Users.ViewModels.UserViewModels
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {
            Roles = new HashSet<string>();
            Groups = new HashSet<string>();
            Tenant = new Tenant();
        }

        public Guid UserId { get; set; }

        [Display(Name = "User Name", Description = "user name", Prompt = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "Email Address :", Description = "email", Prompt = "email")]
        public string Email { get; set; }

        [Display(Name = "First name :", Description = "first name", Prompt = "ex: John")]
        public string UserFirstName { get; set; }

        [Display(Name = "Last name :", Description = "last name", Prompt = "ex: Doe")]
        public string UserLastName { get; set; }

        [Display(Name = "Last name :", Description = "last name", Prompt = "ex: Doe")]
        public string UserPhoneNumber { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime Birthday { get; set; }
        [MaxLength(500)] public string AboutMe { get; set; }

        public Tenant Tenant { get; set; }


        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Groups { get; set; }
    }
}