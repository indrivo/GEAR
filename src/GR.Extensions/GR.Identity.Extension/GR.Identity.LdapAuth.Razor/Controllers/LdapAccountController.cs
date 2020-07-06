using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Identity.Abstractions.ViewModels.AccountViewModels;
using GR.Identity.LdapAuth.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.LdapAuth.Razor.Controllers
{
    [Authorize]
    public class LdapAccountController : Controller
    {
        /// <summary>
        /// Return url
        /// </summary>
        private const string ReturnUrl = "ReturnUrl";

        #region Injectable

        /// <summary>
        /// Inject ad auth service
        /// </summary>
        private readonly ILdapAuthorizeService _ldapAuthorizeService;

        #endregion

        public LdapAccountController(ILdapAuthorizeService ldapAuthorizeService)
        {
            _ldapAuthorizeService = ldapAuthorizeService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData[ReturnUrl] = returnUrl;
            return View();
        }

        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);
            var authResult = await _ldapAuthorizeService.LoginAsync(model);
            if (authResult.IsSuccess)
            {
                return RedirectToLocal(returnUrl);
            }

            ModelState.AppendResultModelErrors(authResult.Errors);
            return View(model);
        }


        #region Helpers

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        #endregion Helpers
    }
}