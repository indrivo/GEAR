using GR.Identity.Abstractions.Models.Permmisions;
using GR.Identity.Abstractions.Models.UserProfiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Roles.Razor.ViewModels.RoleViewModels
{
    public class UpdateRoleViewModel
    {
        public IEnumerable<Profile> Profiles { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }

        [Display(Name = "Permission")]
        public List<string> SelectedPermissionId { get; set; }

        [Display(Name = "User's profiles")]
        public List<string> SelectedProfileId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Id { get; set; }

        public bool IsDeleted { get; set; }

        [Required, StringLength(50)]
        public string Title { get; set; }

        public bool IsNoEditable { get; set; }

        public string ClientName { get; set; }

        public Guid? TenantId { get; set; }
    }
}