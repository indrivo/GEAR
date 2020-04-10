using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Identity.Permissions.Abstractions.Permissions;

namespace GR.Identity.Clients.Razor.ViewModels.RoleViewModels
{
    public class UpdateRoleViewModel
    {
        public IEnumerable<Permission> Permissions { get; set; }

        [Display(Name = "Permission")]
        public List<string> SelectedPermissionId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required]
        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        [Required, StringLength(50)]
        public string Title { get; set; }

        public bool IsNoEditable { get; set; }

        public string ClientName { get; set; }

        public Guid? TenantId { get; set; }
    }
}