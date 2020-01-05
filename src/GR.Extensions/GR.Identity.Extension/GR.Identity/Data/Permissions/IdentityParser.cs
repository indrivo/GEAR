using GR.Identity.Abstractions;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace GR.Identity.Data.Permissions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IdentityParser : IIdentityParser<GearUser>
    {
        private readonly RoleManager<GearRole> _roleManager;
        private readonly UserManager<GearUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="roleManager"></param>
        /// <param name="userManager"></param>
        public IdentityParser(RoleManager<GearRole> roleManager, UserManager<GearUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Check claims compatibility
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        private static bool IsClaimOfType(Claim claim, string claimType)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (string.IsNullOrEmpty(claimType))
                throw new ArgumentNullException(nameof(claimType));

            return claim.Type == claimType;
        }

        /// <summary>
        /// Parse claims
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task<GearUser> Parse(IPrincipal principal)
        {
            if (!(principal is ClaimsPrincipal claimsPrincipal))
                throw new ArgumentException("The principal must be a ClaimsPrincipal", nameof(principal));
            var claims = claimsPrincipal.Claims.ToList();
            var userFromClaims = new GearUser
            {
                Id = claims.FirstOrDefault(claim => IsClaimOfType(claim, JwtClaimTypes.Subject))?.Value ?? string.Empty,
                UserName = claims.FirstOrDefault(claim => IsClaimOfType(claim, JwtClaimTypes.Name))?.Value ?? string.Empty,
                Email = claims.FirstOrDefault(claim => IsClaimOfType(claim, JwtClaimTypes.Email))?.Value ?? string.Empty,
            };
            var user = await _userManager.FindByNameAsync(userFromClaims.UserName);
            return user;
        }
    }
}