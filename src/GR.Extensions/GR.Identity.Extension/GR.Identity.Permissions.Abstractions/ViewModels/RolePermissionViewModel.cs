using System.Collections.Generic;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Permissions;

namespace GR.Identity.Permissions.Abstractions.ViewModels
{
    public class RolePermissionViewModel : GearRole
    {
        /// <summary>
        /// List of permissions
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}