using System;
using System.Collections.Generic;
using System.Text;

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
