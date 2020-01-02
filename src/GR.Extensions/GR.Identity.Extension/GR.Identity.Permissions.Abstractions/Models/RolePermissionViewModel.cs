using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.Permmisions;
using System.Collections.Generic;

namespace GR.Identity.Permissions.Abstractions.Models
{
    public class RolePermissionViewModel : GearRole
    {
        /// <summary>
        /// List of permissions
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}