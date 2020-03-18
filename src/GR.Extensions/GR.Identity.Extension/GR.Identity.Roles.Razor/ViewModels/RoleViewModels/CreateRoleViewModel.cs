using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Identity.Permissions.Abstractions.Permissions;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace GR.Identity.Roles.Razor.ViewModels.RoleViewModels
{
    public class CreateRoleViewModel
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

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