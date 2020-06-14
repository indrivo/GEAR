using System;
using System.Collections.Generic;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GR.Identity.Abstractions.Helpers.Attributes
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class GearAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Roles shortcut
        /// </summary>
        /// <param name="roles"></param>
        public GearAuthorizeAttribute(params string[] roles)
        {
            Roles = string.Join(",", roles);
        }

        /// <summary>
        /// Schemes shortcut
        /// </summary>
        /// <param name="authenticationSchemes"></param>
        public GearAuthorizeAttribute(GearAuthenticationScheme authenticationSchemes)
        {
            AuthenticationSchemes = GetAuthenticationSchemes(authenticationSchemes);
        }

        /// <summary>
        /// Get authentication schemes
        /// </summary>
        /// <param name="authenticationSchemes"></param>
        /// <returns></returns>
        private string GetAuthenticationSchemes(GearAuthenticationScheme authenticationSchemes)
        {
            var flags = new List<GearAuthenticationScheme>();
            if ((authenticationSchemes & GearAuthenticationScheme.Identity) == GearAuthenticationScheme.Identity)
            {
                flags.Add(GearAuthenticationScheme.Identity);
            }

            if ((authenticationSchemes & GearAuthenticationScheme.Bearer) == GearAuthenticationScheme.Bearer)
            {
                flags.Add(GearAuthenticationScheme.Bearer);
            }

            if ((authenticationSchemes & GearAuthenticationScheme.Cookies) == GearAuthenticationScheme.Cookies)
            {
                flags.Add(GearAuthenticationScheme.Cookies);
            }

            if ((authenticationSchemes & GearAuthenticationScheme.IdentityWithBearer) == GearAuthenticationScheme.IdentityWithBearer)
            {
                flags.Add(GearAuthenticationScheme.IdentityWithBearer);
            }

            var schemes = new List<string>();
            foreach (var flag in flags)
            {
                switch (flag)
                {
                    case GearAuthenticationScheme.Identity:
                        schemes.Add(IdentityConstants.ApplicationScheme);
                        break;
                    case GearAuthenticationScheme.Bearer:
                        schemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        break;
                    case GearAuthenticationScheme.IdentityWithBearer:
                        schemes.Add(IdentityConstants.ApplicationScheme + "," + JwtBearerDefaults.AuthenticationScheme);
                        break;
                    case GearAuthenticationScheme.Cookies:
                        schemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(authenticationSchemes), authenticationSchemes, null);
                }
            }
            var authSchemes = string.Join(",", schemes);
            return authSchemes;
        }
    }

    [Flags]
    public enum GearAuthenticationScheme
    {
        Identity = 10,
        Bearer = 11,
        IdentityWithBearer = 12,
        Cookies = 13
    }
}