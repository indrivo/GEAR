using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GR.Identity.Permissions.Abstractions
{
    public interface IPermissionService
    {
        /// <summary>
        /// Has permission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> HasClaimAsync(Guid userId, string permission);

        /// <summary>
        /// Has permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> HasClaimAsync(GearUser user, string permission);

        /// <summary>
        /// Has permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasClaim(ClaimsPrincipal user, string permission);

        /// <summary>
        /// Get user claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Claim>> GetUserClaimsAsync(Guid userId);

        /// <summary>
        /// Get roles permissions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RolePermissionViewModel>> RolesPermissionsAsync();

        /// <summary>
        /// Refresh cache
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, IEnumerable<string>>> SetOrResetPermissionsOnCacheAsync();

        /// <summary>
        /// Check if user have permission
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="userPermissions"></param>
        /// <returns></returns>
        Task<bool> HasPermissionAsync(IList<string> roles, IList<string> userPermissions);

        /// <summary>
        /// Refresh cache by role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        Task RefreshCacheByRoleAsync(string roleName, bool delete = false);

        /// <summary>
        /// Check for permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<bool> HasPermissionAsync(IList<string> permissions);
    }
}