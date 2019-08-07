using System.Collections.Generic;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models.Permmisions;

namespace ST.Identity.Permissions.Abstractions.Models
{
    public class RolePermissionViewModel : ApplicationRole
    {
        /// <summary>
        /// List of permissions
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
