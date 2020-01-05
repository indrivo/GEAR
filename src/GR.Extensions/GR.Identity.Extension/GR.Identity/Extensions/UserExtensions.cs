using GR.Identity.Abstractions;
using GR.Identity.Data;
using GR.Identity.Permissions.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GR.Identity.Extensions
{
    public static class UserExtensions
    {
        /// <summary>
        /// Get claims from permissions
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IList<Claim>> GetClaimsFromPermissions<TContext>(this GearUser user,
            TContext context) where TContext : ApplicationDbContext
        {
            var userPermission = new HashSet<string>();
            var roles = await context.UserRoles.Where(x => x.UserId.Equals(user.Id))
                .Select(x => x.RoleId).ToListAsync();

            foreach (var _ in roles)
            {
                await context.RolePermissions.Where(x => x.RoleId.Equals(_)).ForEachAsync(
                    async x =>
                    {
                        await context.Permissions.Where(l => l.Id.Equals(x.PermissionId))
                            .ForEachAsync(y => userPermission.Add(y.PermissionKey));
                    });
            }

            return userPermission.Select(x => new Claim(x, x)).ToList();
        }

        /// <summary>
        /// Refresh claims in database for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="signInManager"></param>
        /// <returns></returns>
        public static async Task RefreshClaims<TContext>(this GearUser user, TContext context,
            SignInManager<GearUser> signInManager) where TContext : ApplicationDbContext
        {
            var oldClaims = await signInManager.UserManager.GetClaimsAsync(user);
            var claims = await user.GetClaimsFromPermissions(context);
            var listOfNewClaims = claims.Where(cl => !oldClaims.Select(x => x.Type).Contains(cl.Type)).ToList();
            await signInManager.UserManager.AddClaimsAsync(user, listOfNewClaims);
            var listOfOldClaims = oldClaims.Where(oldCl => !claims.Select(x => x.Type).Contains(oldCl.Type)).ToList();
            await signInManager.UserManager.RemoveClaimsAsync(user, listOfOldClaims);

            await signInManager.RefreshSignInAsync(user);
        }

        /// <summary>
        /// Refresh claims for users online
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <param name="signInManager"></param>
        /// <param name="onlineUsers"></param>
        /// <returns></returns>
        public static async Task RefreshOnlineUsersClaims<TContext>(this ClaimsPrincipal user, TContext context,
            SignInManager<GearUser> signInManager, IEnumerable<Guid> onlineUsers) where TContext : ApplicationDbContext
        {
            foreach (var onlineUser in onlineUsers)
            {
                var usr = context.Users.FirstOrDefault(x => x.Id.Equals(onlineUser.ToString()));
                if (usr != null)
                {
                    await usr.RefreshClaims(context, signInManager);
                }
            }
        }

        /// <summary>
        /// User has permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permissionService"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static async Task<bool> HasPermission(this ClaimsPrincipal user, IPermissionService permissionService, string permission)
        {
            var roles = user.Claims.Where(x => x.Type.Equals("role") || x.Type.EndsWith("role")).Select(x => x.Value)
                .ToList();
            return await permissionService.HasPermissionAsync(roles, new List<string> { permission });
        }

        /// <summary>
        /// User ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                return null;

            ClaimsPrincipal currentUser = user;
            return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}