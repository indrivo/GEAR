using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace ST.Identity.Roles.Razor.ViewModels.RoleViewModels
{
	public class CreateRoleViewModel
	{
		public IEnumerable<Profile> Profiles { get; set; }

		[Required, StringLength(50)]
		public string Name { get; set; }

		[Display(Name = "User's profiles")]
		public List<string> SelectedProfileId { get; set; }

		public bool IsDeleted { get; set; }

		[Required, StringLength(50)]
		public string Title { get; set; }

		[Required]
		[Display(Name = "Client Id")]
		public int ClientId { get; set; }

		public IEnumerable<Client> Clients { get; set; }

		public IEnumerable<Permission> PermissionsList { get; set; }

		[Display(Name = "Permissions list")]
		public List<string> SelectedPermissionId { get; set; }
	}
}