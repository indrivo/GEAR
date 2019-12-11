using System;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.MultiTenant.Razor.ViewModels;

namespace GR.MultiTenant.Razor.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        /// <summary>
        /// Inject http context
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        #endregion


        public CompanyController(IStringLocalizer localizer, IHttpContextAccessor httpContextAccessor, IUserManager<GearUser> userManager, SignInManager<GearUser> signInManager)
        {
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confirmToken"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(Guid? userId, string confirmToken)
        {
            if (!userId.HasValue || string.IsNullOrEmpty(confirmToken))
            {
                return NotFound();
            }

            if (!_httpContextAccessor.HttpContext.User.IsAuthenticated())
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
            }

            var currentUser = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
            if (currentUser == null)
            {
                return NotFound();
            }

            var confirmEmailResult = await _userManager.UserManager.ConfirmEmailAsync(currentUser, confirmToken);
            if (!confirmEmailResult.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(currentUser, true);
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// View for confirm invited user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="confirmToken"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmInvitedUserByEmail(Guid? userId, string confirmToken)
        {
            if (!userId.HasValue || string.IsNullOrEmpty(confirmToken))
            {
                return NotFound();
            }

            if (!_httpContextAccessor.HttpContext.User.IsAuthenticated())
            {
                await _httpContextAccessor.HttpContext.SignOutAsync();
            }

            var currentUser = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
            if (currentUser == null)
            {
                return NotFound();
            }

            var model = new ConfirmEmailViewModel
            {
                UserId = currentUser.Id,
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Token = confirmToken
            };
            return View(model);
        }

        /// <summary>
        /// Save password for new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmInvitedUserByEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(model.UserId));
            var resetToken = await _userManager.UserManager.GeneratePasswordResetTokenAsync(user);
            if (resetToken == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["system_error_on_generate_reset_token"]);
                return View(model);
            }

            var result = await _userManager.UserManager.ResetPasswordAsync(user, resetToken, model.Password);
            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
                if (confirmEmailToken == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["system_error_on_generate_token"]);
                    return View(model);
                }

                var confirmEmailResult = await _userManager.UserManager.ConfirmEmailAsync(user, confirmEmailToken);
                if (!confirmEmailResult.Succeeded)
                {
                    ModelState.AppendIdentityResult(confirmEmailResult);
                    return View(model);
                }

                await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AppendIdentityResult(result);
            return View(model);
        }
    }
}