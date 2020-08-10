using System.Threading.Tasks;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Clients.Razor.Api
{
    [AllowAnonymous]
    [Route(DefaultApiRouteTemplate)]
    public class OpenIdApiController : BaseGearController
    {
        /// <summary>
        /// Get identityServer4 discovery document
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DiscoveryDocument()
        {
            return Redirect("/.well-known/openid-configuration");
        }

        /// <summary>
        /// Authenticate and redirect
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userManager"></param>
        /// <param name="authorizeService"></param>
        /// <returns></returns>
        [HttpGet]
        [GearAuthorize(GearAuthenticationScheme.Bearer)]
        public async Task<IActionResult> AuthenticateCookieFromTokenAndRedirect(string url, [FromServices] IUserManager<GearUser> userManager, [FromServices] IAuthorizeService authorizeService)
        {
            var user = (await userManager.GetCurrentUserAsync()).Result;
            var auth = await authorizeService.LoginAsync(user);
            if (!auth.IsSuccess)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return Redirect(url);
        }
    }
}
