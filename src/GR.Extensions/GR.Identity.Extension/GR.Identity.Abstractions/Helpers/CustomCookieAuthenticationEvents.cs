using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

namespace GR.Identity.Abstractions.Helpers
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<CustomCookieAuthenticationEvents> _logger;

        #endregion

        public CustomCookieAuthenticationEvents(IUserManager<GearUser> userManager, ILogger<CustomCookieAuthenticationEvents> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (!(context.Principal?.Identity is ClaimsIdentity claimIdentity)) return;

            var userIdClaim = claimIdentity.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (userIdClaim.IsNullOrEmpty()) return;

            var securityStamp = await _userManager.GetSecurityStampAsync(userIdClaim.ToGuid());
            if (!securityStamp.IsSuccess) return;

            var claimSecurityStamp = claimIdentity.Claims.FirstOrDefault(c => c.Type == "AspNet.Identity.SecurityStamp")?.Value;
            if (claimSecurityStamp != securityStamp.Result)
            {
                _logger.LogWarning("Security stamp differ for user: {User}, principal is rejected, sign out will be made", userIdClaim);
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync();
            }
        }
    }
}