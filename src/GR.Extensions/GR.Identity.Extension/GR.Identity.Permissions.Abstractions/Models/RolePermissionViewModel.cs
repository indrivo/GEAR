using System.Collections.Generic;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.Permmisions;

namespace GR.Identity.Permissions.Abstractions.Models
{
    public class RolePermissionViewModel : ApplicationRole
    {
        /// <summary>
        /// List of permissions
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
