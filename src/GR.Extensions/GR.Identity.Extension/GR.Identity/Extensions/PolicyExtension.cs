using GR.Identity.Data.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace GR.Identity.Extensions
{
    public static class PolicyExtension
    {
        /// <summary>
        /// Get policies from permissions
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static AuthorizationOptions GetPoliciesFromPermissions(this AuthorizationOptions options)
        {
            foreach (var _ in PermissionsConstants.PermissionsList())
            {
                options.AddPolicy(_, policy => policy.RequireClaim(_));
            }
            return options;
        }
    }
}