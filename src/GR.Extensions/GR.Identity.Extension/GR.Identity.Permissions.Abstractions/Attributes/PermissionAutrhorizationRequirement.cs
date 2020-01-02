using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace GR.Identity.Permissions.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Requirements class
    /// </summary>
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// List of Required Permissions
        /// </summary>
        public IEnumerable<string> RequiredPermissions { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requiredPermissions">List of required Permissions</param>
        public PermissionAuthorizationRequirement(IEnumerable<string> requiredPermissions)
        {
            RequiredPermissions = requiredPermissions;
        }
    }
}