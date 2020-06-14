using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.Identity.Abstractions;
using GR.Identity.Clients.Abstractions.Helpers;
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
    [Author(Authors.LUPEI_NICOLAE)]
    public class ProfileService : IProfileService
    {
        #region DependencyInjection Private Fields

        private readonly UserManager<GearUser> _userManager;

        #endregion 

        public ProfileService(UserManager<GearUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get profile data
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject ?? throw new ArgumentNullException(nameof(context.Subject));
            var subjectId = subject.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(subjectId))
                throw new ArgumentException("Invalid subject id", nameof(subjectId));

            var user = await _userManager.FindByIdAsync(subjectId);
            var claims = GetUserClaims(user, context).ToList();
            context.IssuedClaims = claims;
        }

        /// <summary>
        /// Check if is active
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task IsActiveAsync(IsActiveContext context)
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

                if (user.IsDeleted || user.IsPasswordExpired())
                {
                    context.IsActive = false;
                }
                else
                {
                    context.IsActive =
                        !user.LockoutEnabled ||
                        !user.LockoutEnd.HasValue ||
                        user.LockoutEnd <= DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Get user claims
        /// </summary>
        /// <param name="user"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Claim> GetUserClaims(GearUser user, ProfileDataRequestContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(GearClaimTypes.Tenant, user.TenantId?.ToString() ?? string.Empty),
                new Claim(GearClaimTypes.IsDisabled, user.IsDisabled.ToString()),
                new Claim(GearClaimTypes.UserPhotoUrl, $"/Users/GetImage?id={user.Id}"),
                new Claim(GearClaimTypes.BirthDay, user.Birthday.ToString(CultureInfo.InvariantCulture))
            };
            var identityRequestResources = context.RequestedResources?.IdentityResources?.Select(x => x.Name).ToList() ?? new List<string>();
            if (identityRequestResources.Contains("email"))
            {
                claims.Add(new Claim("mail", user.Email ?? string.Empty));
            }

            if (identityRequestResources.Contains("phone"))
            {
                claims.Add(new Claim("phone", user.PhoneNumber ?? string.Empty));
            }

            if (identityRequestResources.Contains("profile"))
            {
                claims.Add(new Claim(GearClaimTypes.FirstName, user.FirstName ?? string.Empty));
                claims.Add(new Claim(GearClaimTypes.LastName, user.LastName ?? string.Empty));
            }

            if (!_userManager.SupportsUserRole) return claims;

            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            return claims;
        }
    }
}