using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Templates;
using GR.Email.Abstractions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Authorization;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Data;
using GR.Identity.Razor.Extensions;
using GR.Identity.Razor.ViewModels.AccountViewModels;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GR.Identity.Razor.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Private Dependency Injection Fields

        /// <summary>
        /// Inject email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject interaction service
        /// </summary>
        private readonly IIdentityServerInteractionService _interactionService;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Inject SignIn Manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        /// <summary>
        /// Inject accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccesor;

        /// <summary>
        /// Inject app context
        /// </summary>
        private readonly GearIdentityDbContext _identityDbContext;

        #endregion

        /// <summary>
        /// Return url
        /// </summary>
        private const string ReturnUrl = "ReturnUrl";

        public AccountController(
            SignInManager<GearUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IIdentityServerInteractionService interactionService,
            IUserManager<GearUser> userManager,
            IHttpContextAccessor httpContextAccesor,
            GearIdentityDbContext identityDbContext)
        {
            _httpContextAccesor = httpContextAccesor;
            _identityDbContext = identityDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _interactionService = interactionService;
        }

        /// <summary>
        /// Error message
        /// </summary>
        [TempData]
        private string ErrorMessage { get; set; }

        /// <summary>
        /// Get view for access denied
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = nameof(AccessDenied);
            return View();
        }

        /// <summary>
        /// External auth
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model,
            string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }

                var webClient = new WebClient();
                var imageBytes = await webClient.DownloadDataTaskAsync(model.Picture);
                var user = new GearUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    Created = DateTime.Now,
                    UserPhoto = imageBytes,
                };

                var result = await _userManager.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }

                ModelState.AppendIdentityResult(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        /// <summary>
        /// Get view for forgot password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid model"));
                return Json(resultModel);
            }

            var user = await _userManager.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found"));
                return Json(resultModel);
            }

            if (!await _userManager.UserManager.IsEmailConfirmedAsync(user))
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Email is not confirmed"));
                return Json(resultModel);
            }

            var code = await _userManager.UserManager.GeneratePasswordResetTokenAsync(user);
            if (code == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Error on generate reset token, try again"));
                return Json(resultModel);
            }

            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme);
            var mail = $"Please reset your password by clicking here : <a href='{callbackUrl}'>link</a>";
            var templateRequest = TemplateManager.GetTemplateBody("forgot-password");
            if (templateRequest.IsSuccess)
            {
                mail = templateRequest.Result?.Inject(new Dictionary<string, string>
                {
                    { "Link", callbackUrl }
                });
            }
            await _emailSender.SendEmailAsync(model.Email, "Reset Password", mail);

            IdentityEvents.Users.UserForgotPassword(new UserForgotPasswordEventArgs
            {
                Email = model.Email
            });

            resultModel.IsSuccess = true;
            return Json(resultModel);
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
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
            ViewData[ReturnUrl] = returnUrl == Url.Action("LocalLogout")
                ? Url.Action("Index", "Home")
                : returnUrl;

            if (!ModelState.IsValid) return View(model);

            var user = _userManager.UserManager.Users.FirstOrDefault(x => x.UserName == model.Email);
            if (user != null)
            {
                if (user.IsDeleted)
                {
                    ModelState.AddModelError(string.Empty, "User is disabled by admin.");
                    return View(model);
                }

                if (user.IsPasswordExpired() && !await _userManager.UserManager.IsInRoleAsync(user, GlobalResources.Roles.ADMINISTRATOR))
                {
                    ModelState.AddModelError(string.Empty,
                        "Password has been expired, you need to change the password");
                    return View(model);
                }

                await ClearUserClaims(user);
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                        false);

                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.Now;
                    await _userManager.UserManager.UpdateAsync(user);
                    //Sync permissions to claims
                    //await user.RefreshClaims(_applicationDbContext, _signInManager);
                    _logger.LogInformation("User logged in.");

                    IdentityEvents.Authorization.UserLogin(new UserLogInEventArgs
                    {
                        IpAdress = _userManager.GetRequestIpAdress(),
                        UserId = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });

                    var claim = new Claim(nameof(Tenant).ToLowerInvariant(), user.TenantId.ToString());

                    await _userManager.UserManager.AddClaimAsync(user, claim);
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid credentials!");
            return View(model);
        }

        /// <summary>
        /// Clear user claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [NonAction]
        private async Task ClearUserClaims(GearUser user)
        {
            var userClaims = await _identityDbContext.UserClaims.Where(x => x.UserId == user.Id).ToListAsync();
            if (userClaims.Any())
            {
                _identityDbContext.UserClaims.RemoveRange(userClaims);
                await _identityDbContext.SaveAsync();
            }
        }

        /// <summary>
        /// Handle logout page post-back
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we sign-out and redirect away to the external IdP for sign-out
                    model.LogoutId = await _interactionService.CreateLogoutContextAsync();
                }

                var url = $"/Account/Logout?logoutId={model.LogoutId}";

                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.SignOutAsync(idp, new AuthenticationProperties
                    {
                        RedirectUri = url
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.Message);
                }
            }

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interactionService.GetLogoutContextAsync(model.LogoutId);

            return Redirect(logout?.PostLogoutRedirectUri);
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        /// <param name="logoutId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (string.IsNullOrEmpty(logoutId))
                await _signInManager.SignOutAsync();

            if (User.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            //Test for Xamarin.
            var context = await _interactionService.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                //it's safe to automatically sign-out
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };
            return View(vm);
        }

        /// <summary>
        /// Local logout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> LocalLogout([FromServices] IUserManager<GearUser> manager)
        {
            var result = new ResultModel();
            var userReq = await manager.GetCurrentUserAsync();
            if (!userReq.IsSuccess)
            {
                result.Errors.Add(new ErrorModel(nameof(AuthorizationFailure), "Error on logout!!"));
                return Json(result);
            }

            var user = userReq.Result;

            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                result.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
                return Json(result);
            }

            IdentityEvents.Authorization.UserLogout(new UserLogOutEventArgs
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IpAdress = _userManager.GetRequestIpAdress()
            });

            result.IsSuccess = true;
            return Json(result);
        }

        /// <summary>
        /// Get view for create new user
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData[ReturnUrl] = returnUrl;
            return View();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData[ReturnUrl] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var user = new GearUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                await _signInManager.SignInAsync(user, false);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToLocal(returnUrl);
            }

            this.AddIdentityErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(Guid userId, string code)
        {
            if (string.IsNullOrEmpty(code) || userId != Guid.Empty)
            {
                NotFound();
            }

            var user = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(model);
            }

            var result = await _userManager.UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                user.LastPasswordChanged = DateTime.Now;
                await _userManager.UserManager.UpdateAsync(user);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            this.AddIdentityErrors(result);
            return View(model);
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetCurrentUser()
        {
            var user = await _userManager.UserManager.GetUserAsync(User);
            return Json(new ResultModel
            {
                IsSuccess = user != null,
                Result = user
            });
        }

        /// <summary>
        /// Get token for be used in ajax requests
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetToken()
        {
            var context = _httpContextAccesor.HttpContext;
            var token = await context.GetTokenAsync("access_token");
            return Json(token);
        }

        /// <summary>
        /// External login
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// External login call back
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="remoteError"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result =
                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            var identifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var picture = string.Empty;

            switch (info.LoginProvider)
            {
                case "Facebook":
                    picture = $"https://graph.facebook.com/{identifier}/picture?type=large";
                    break;

                case "Google":
                    var index = name.LastIndexOf("(", StringComparison.Ordinal);
                    if (index > 0)
                    {
                        name = name.Substring(0, index);
                    }

                    picture = info.Principal.FindFirstValue("image");
                    break;

                case "LinkedIn":
                    picture = info.Principal.FindFirstValue("image");
                    break;
            }

            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            return View("ExternalLogin",
                new ExternalLoginViewModel
                {
                    Email = email,
                    Name = name.Replace(" ", string.Empty),
                    Picture = picture,
                    LoginProvider = info.LoginProvider
                });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
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