using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Enums;
using ST.Identity.Data.MultiTenants;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.Razor.Users.ViewModels.UserViewModels
{
	public class CreateUserViewModel
	{
		public IEnumerable<ApplicationRole> Roles { get; set; }
		public IEnumerable<AuthGroup> Groups { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Select a role for this user"),
		 Display(Name = "User's Role")]
		public List<string> SelectedRoleId { get; set; }

		[Display(Name = "User's group")]
		public List<string> SelectedGroupId { get; set; }


		[Required]
		[Display(Name = "User Name")]
		[Remote(action: "VerifyName", controller: "Users")]
		public string UserName { get; set; }

		[Display(Name = "Is Deleted")]
		public bool IsDeleted { get; set; }

		[Required,
		 StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
			 MinimumLength = 6), DataType(DataType.Password)]
		public string Password { get; set; }

		[Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords should match")]
		[Display(Name = "Repeat Password")]
		public string RepeatPassword { get; set; }

		[Required, EmailAddress] public string Email { get; set; }

		public List<EntityViewModel> Profiles { get; set; }
		/// <summary>
		/// Authentication Type
		/// </summary>
		[Required]
		[Display(Name = "Select Authentication Type")]
		public AuthenticationType AuthenticationType { get; set; }
		public string ProfilesJson { get; set; }

		[Display(Name = "User Photo")]
		public IFormFile UserPhoto { get; set; }

		/// <summary>
		/// List with tenants
		/// </summary>
		public IEnumerable<Tenant> Tenants { get; set; }

		/// <summary>
		/// User organization
		/// </summary>
		[Display(Name = "Select user organization")]
		public Guid? TenantId { get; set; }
	}
}