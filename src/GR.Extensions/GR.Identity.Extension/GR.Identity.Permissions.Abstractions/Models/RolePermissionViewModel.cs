using System.Collections.Generic;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.Permmisions;

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
