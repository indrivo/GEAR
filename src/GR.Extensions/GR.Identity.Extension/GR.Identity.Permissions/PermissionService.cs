using GR.Cache.Abstractions;
using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions;
using GR.Identity.Permissions.Abstractions.Configurators;
using GR.Identity.Permissions.Abstractions.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GR.Identity.Permissions
{
    public class PermissionService<TContext> : IPermissionService where TContext : IIdentityContext
    {
        #region Injectable
        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        /// <summary>
        /// Inject distributed cache
        /// </summary>
        private readonly ICacheService _cache;

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly TContext _context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;
        #endregion

        #region Constants

        /// <summary>
        /// Name of stored role permission in cache
        /// </summary>
        protected const string CacheKeyName = "RolePermissions";

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="cache"></param>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        public PermissionService(SignInManager<GearUser> signInManager, ICacheService cache,
            TContext context, IUserManager<GearUser> userManager)
        {
            _signInManager = signInManager;
            _cache = cache;
            _context = context;
            _userManager = userManager;
        }

        /// <inheritdoc />
        /// <summary>
        /// Has permission for user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public virtual async Task<bool> HasClaimAsync(Guid userId, string permission)
        {
            var claims = await GetUserClaimsAsync(userId);
            return claims.Select(x => x.Type).Contains(permission);
        }

        /// <inheritdoc />
        /// <summary>
        /// Has permission for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public virtual async Task<bool> HasClaimAsync(GearUser user, string permission)
        {
            if (user == null) return false;
            var claims = await _signInManager.UserManager.GetClaimsAsync(user);
            return claims.Select(x => x.Type).Contains(permission);
        }

        /// <inheritdoc />
        /// <summary>
        /// Has permission from ClaimsPrincipal
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public virtual bool HasClaim(ClaimsPrincipal user, string permission)
        {
            return user != null && user.Claims.Select(x => x.Type).Contains(permission);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(Guid userId)
        {
            if (userId == Guid.Empty) return default;
            var user = await _signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
            if (user == null) return default;
            return await _signInManager.UserManager.GetClaimsAsync(user);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get roles permissions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<RolePermissionViewModel>> RolesPermissionsAsync()
        {
            var result = new List<RolePermissionViewModel>();
            var roles = await _context.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var data = role.Adapt<RolePermissionViewModel>();
                var permissions = await _context.RolePermissions.Where(x => x.RoleId.Equals(role.Id))
                    .Select(c => _context.Permissions.FirstOrDefault(g => g.Id.Equals(c.PermissionId)))
                    .ToListAsync();
                data.Permissions = permissions;
                result.Add(data);
            }

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Refresh Permission for roles
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, IEnumerable<string>>> SetOrResetPermissionsOnCacheAsync()
        {
            var roles = await RolesPermissionsAsync();
            var store = roles.ToDictionary(role => role.Name, role => role.Permissions.Select(x => x.PermissionKey));
            await _cache.SetAsync(CacheKeyName, store);
            return store;
        }

        /// <inheritdoc />
        /// <summary>
        /// Refresh cache by role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public virtual async Task RefreshCacheByRoleAsync(string roleName, bool delete = false)
        {
            var storeDictionary = await _cache.GetAsync<Dictionary<string, IEnumerable<string>>>(CacheKeyName);
            if (delete)
            {
                if (storeDictionary.ContainsKey(roleName))
                {
                    storeDictionary.Remove(roleName);
                }

                return;
            }

            var role = await _context.SetEntity<GearRole>().FirstOrDefaultAsync(x => x.Name.Equals(roleName));

            if (role == null)
            {
                return;
            }

            var data = role.Adapt<RolePermissionViewModel>();
            var permissions = await _context.RolePermissions.Where(x => x.RoleId.Equals(role.Id))
                .Select(c => _context.Permissions.FirstOrDefault(g => g.Id.Equals(c.PermissionId)))
                .ToListAsync();

            data.Permissions = permissions;

            if (storeDictionary.ContainsKey(data.Name))
            {
                storeDictionary[data.Name] = data.Permissions.Select(x => x.PermissionKey);
            }
            else
            {
                storeDictionary.Add(data.Name, data.Permissions.Select(x => x.PermissionKey));
            }

            await _cache.SetAsync(CacheKeyName, storeDictionary);
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if user have permission
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="userPermissions"></param>
        /// <returns></returns>
        public virtual async Task<bool> HasPermissionAsync(IList<string> roles, IList<string> userPermissions)
        {
            var match = new List<string>();
            if (!userPermissions.Any() || !roles.Any()) return false;
            var data = await _cache.GetAsync<Dictionary<string, IEnumerable<string>>>(CacheKeyName) ?? await SetOrResetPermissionsOnCacheAsync();
            try
            {
                foreach (var role in data)
                {
                    if (!roles.Contains(role.Key)) continue;
                    foreach (var perm in userPermissions)
                    {
                        if (!role.Value.Contains(perm)) continue;
                        if (!match.Contains(perm)) match.Add(perm);
                    }

                    if (!match.Count.Equals(userPermissions.Count)) continue;
                    var userRequest = await _userManager.GetCurrentUserAsync();
                    if (!userRequest.IsSuccess) return true;
                    var user = userRequest.Result;
                    return PermissionCustomRules.ExecuteRulesAndCheckAccess(userPermissions, roles, user.TenantId, user.Id.ToGuid());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// Check for permissions
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual async Task<bool> HasPermissionAsync(IList<string> permissions)
        {
            if (!permissions.Any()) return false;
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return false;
            var roles = await _userManager.UserManager.GetRolesAsync(currentUserRequest.Result);
            return await HasPermissionAsync(roles, permissions);
        }
    }
}