using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Identity.Abstractions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace GR.Identity.Clients.Infrastructure
{
    /// <summary>
    /// Returns the profile data associated to the main
    /// Identity of the user
    /// </summary>
    public class ProfileService : IProfileService
    {
        #region DependencyInjection Private Fields

        private readonly UserManager<GearUser> _userManager;

        #endregion DependencyInjection Private Fields

        public ProfileService(UserManager<GearUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = subject.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(subjectId))
                throw new ArgumentException("Invalid subject id", nameof(subjectId));

            var user = await _userManager.FindByIdAsync(subjectId);
            var claims = GetUserClaims(user, _userManager).ToList();
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = subject.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Subject)?.Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var securityStamp = subject.Claims
                        .Where(claim => claim.Type == "security_stamp")
                        .Select(claim => claim.Value)
                        .SingleOrDefault();

                    if (securityStamp != null)
                    {
                        var dbSecurityStamp = await _userManager.GetSecurityStampAsync(user);
                        if (dbSecurityStamp != securityStamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.UtcNow;
            }
        }

        private static IEnumerable<Claim> GetUserClaims(GearUser user, UserManager<GearUser> userManager)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.UserName)
            };

            if (!userManager.SupportsUserRole) return claims;

            var roles = userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            return claims;
        }
    }
}