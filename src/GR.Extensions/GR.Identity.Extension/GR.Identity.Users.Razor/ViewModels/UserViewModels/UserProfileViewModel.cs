using GR.Identity.Abstractions.Models.MultiTenants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Razor.Users.ViewModels.UserViewModels
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

        public Tenant Tenant { get; set; }

        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Groups { get; set; }
    }
}