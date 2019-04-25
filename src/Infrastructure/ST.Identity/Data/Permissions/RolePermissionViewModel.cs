using System.Collections.Generic;

namespace ST.Identity.Data.Permissions
{
    public class RolePermissionViewModel : ApplicationRole
    {
        /// <summary>
        /// List of permissions
        /// </summary>
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
