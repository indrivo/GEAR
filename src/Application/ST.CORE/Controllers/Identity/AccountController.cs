using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.BaseBusinessRepository;
using ST.CORE.Extensions;
using ST.CORE.Models.AccountViewModels;
using ST.CORE.Services;
using ST.Entities.Models.Notifications;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Extensions;
using ST.Identity.LDAP.Services;
using ST.MPass.Gov;
using ST.Notifications.Abstraction;

namespace ST.CORE.Controllers.Identity
{
	[Authorize]
	public class AccountController : Controller
	{
		#region Private Dependency Injection Fields
		/// <summary>
		/// Inject distributed cache from redis 
		/// </summary>
		private readonly IDistributedCache _cache;
		private readonly IEmailSender _emailSender;
		private readonly IIdentityServerInteractionService _interactionService;

		/// <summary>
		/// Inject logger
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// Inject notifier
		/// </summary>
		private readonly INotify _notify;

		private readonly IOptions<MPassOptions> _mpassOptions;
		/// <summary>
		/// Inject M pass service
		/// </summary>
		private readonly IMPassService _mpassService;
		private readonly IMPassSigningCredentialsStore _mpassSigningCredentialStore;
		/// <summary>
		/// Inject SignIn Manager
		/// </summary>
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccesor;
		private readonly ApplicationDbContext _applicationDbContext;
		/// <summary>
		/// Inject Ldap User Manager
		/// </summary>
		private readonly LdapUserManager _ldapUserManager;
		/// <summary>
		/// Inject configured db context
		/// </summary>
		private ConfigurationDbContext ConfigurationDbContext { get; }

		#endregion Private Dependency Injection Fields
		private const string MpassRequestSessionKey = "mpass_request_id";
		private const string ReturnUrl = "ReturnUrl";
		private const string SamlRequest = "SAMLRequest";

		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IEmailSender emailSender,
			ILogger<AccountController> logger,
			IIdentityServerInteractionService interactionService,
			IHttpContextAccessor httpContextAccessor,
			IMPassService mPassService,
			INotify notify,
			IMPassSigningCredentialsStore mpassSigningCredentialStore,
			IOptions<MPassOptions> mpassOptions,
			IDistributedCache distributedCache, IHttpContextAccessor httpContextAccesor, IHostingEnvironment env,
			ApplicationDbContext applicationDbContext, ConfigurationDbContext configurationDbContext, LdapUserManager ldapUserManager)
		{
			_cache = distributedCache;
			_httpContextAccesor = httpContextAccesor;
			_applicationDbContext = applicationDbContext;
			ConfigurationDbContext = configurationDbContext;
			_ldapUserManager = ldapUserManager;
			_mpassOptions = mpassOptions;
			_mpassSigningCredentialStore = mpassSigningCredentialStore;
			_mpassService = mPassService;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_logger = logger;
			_notify = notify;
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
		/// Acs
		/// </summary>
		/// <param name="samlResponse"></param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Acs(string samlResponse)
		{
			var usrInfo = _mpassService.GetMPassUserInfoFromLoginResponse(samlResponse);

			var mpassCredentials = await _mpassSigningCredentialStore.GetMPassCredentialsAsync();
			// NOTE: Keeping InResponseTo in an in-memory Session means this verification will always fail if the web app is restarted during a request
			SamlMessage.LoadAndVerifyLoginResponse(samlResponse, mpassCredentials.IdentityProviderCertificate,
				"http://localhost:9099/Account/Acs", TimeSpan.FromDays(30D),
				usrInfo.RequestId, _mpassOptions.Value.SAMLIssuer, out var ns, out var sessionIndex, out var nameID,
				out var attributes);

			var existingUser = await _userManager.FindByNameAsync(usrInfo.NameID);

			var authProps = new AuthenticationProperties
			{
				AllowRefresh = false,
				ExpiresUtc = DateTimeOffset.UtcNow.AddDays(20),
				IsPersistent = true
			};
			var reqId = _cache.GetString(MpassRequestSessionKey);
			var returnUrl = _cache.GetString($"{reqId}_return_url");

			if (existingUser == null)
				return !string.IsNullOrEmpty(returnUrl)
					? RedirectToLocal(returnUrl)
					: RedirectToAction("Index", "Home");
			_logger.LogInformation("User exists in our database, logging him in");
			await _signInManager.SignInAsync(existingUser, authProps);

			return !string.IsNullOrEmpty(returnUrl) ? RedirectToLocal(returnUrl) : RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Confirm email
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{userId}'.");
			}

			var result = await _userManager.ConfirmEmailAsync(user, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
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
				var user = new ApplicationUser
				{
					UserName = model.Name,
					Email = model.Email,
					Created = DateTime.Now,
					UserPhoto = imageBytes,
				};

				var result = await _userManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await _userManager.AddLoginAsync(user, info);
					if (result.Succeeded)
					{
						await _signInManager.SignInAsync(user, false);
						_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
						return RedirectToLocal(returnUrl);
					}
				}

				AddErrors(result);
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
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
				return RedirectToAction(nameof(ForgotPasswordConfirmation));

			// For more information on how to enable account confirmation and password reset please
			// visit https://go.microsoft.com/fwlink/?LinkID=532713
			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
			await _emailSender.SendEmailAsync(model.Email, "Reset Password",
				$"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
			return RedirectToAction(nameof(ForgotPasswordConfirmation));

			// If we got this far, something failed, redisplay form
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
				return RedirectToAction(nameof(HomeController.Index), "Home");
			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			var requestId = $"_{Guid.NewGuid():n}";

			var samlRequest = SamlMessage.BuildAuthnRequest(requestId, _mpassOptions.Value.SAMLDestination,
				_mpassOptions.Value.SAMLAssertionConsumerUrl, _mpassOptions.Value.SAMLIssuer);
			samlRequest = SamlMessage.Sign(samlRequest,
				_mpassSigningCredentialStore.GetMPassCredentials().ServiceProviderCertificate);
			samlRequest = SamlMessage.Encode(samlRequest);

			_cache.SetString(MpassRequestSessionKey, requestId);
			if (!string.IsNullOrEmpty(returnUrl))
				_cache.SetString($"{requestId}_return_url", returnUrl);

			ViewData[SamlRequest] = samlRequest;
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
			ViewData[ReturnUrl] = returnUrl;
			if (!ModelState.IsValid) return View(model);
			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true

			var user = _userManager.Users.Where(x => x.UserName == model.Email).FirstOrDefault();
			if (user != null && !user.IsDeleted)
			{
				if (user.AuthenticationType == AuthenticationType.Ad)
				{
					var ldapUser = await _ldapUserManager.FindByNameAsync(model.Email);
					if (ldapUser == null)
					{
						ModelState.AddModelError(string.Empty, "Invalid login attempt.");
						return View(model);
					}
					var bind = await _ldapUserManager.CheckPasswordAsync(ldapUser, model.Password);
					if (!bind)
					{
						ModelState.AddModelError(string.Empty, "Invalid credentials.");
						return View(model);
					}

					await _cache.SetStringAsync(user.Id.ToString(),
						Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Password)));

					await _ldapUserManager.ChangeIdentityPassword(user, model.Password);
				}
				var result =
					await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
						false);

				if (result.Succeeded)
				{
					//Sync permissions to claims
					//await user.RefreshClaims(_applicationDbContext, _signInManager);
					_logger.LogInformation("User logged in.");
					await _notify.SendNotificationAsync(new SystemNotifications
					{
						Content = $"User {user.UserName} logged in.",
						Subject = "Info",
						NotificationTypeId = NotificationType.Info
					});
					await _userManager.AddClaimAsync(user, new Claim("tenant", user.TenantId.ToString()));
					return RedirectToLocal(returnUrl);
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}

			ModelState.AddModelError(string.Empty, "User is disabled by admin.");
			return View(model);
		}
		/// <summary>
		/// Use Ldap auth
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		private async Task<ApplicationUser> UseLdapAuth(LoginViewModel model)
		{
			var ldapUser = await _ldapUserManager.FindByNameAsync(model.Email);
			if (ldapUser == null) return null;
			var bind = await _ldapUserManager.CheckPasswordAsync(ldapUser, model.Password);
			if (!bind) return null;
			var exists = await _userManager.FindByNameAsync(ldapUser.Name);
			if (exists != null) return exists;
			//Create new user
			var user = new ApplicationUser
			{
				UserName = ldapUser.SamAccountName,
				Email = ldapUser.EmailAddress,
				AuthenticationType = AuthenticationType.Ad
			};
			var hasher = new PasswordHasher<ApplicationUser>();
			var passwordHash = hasher.HashPassword(user, model.Password);
			user.PasswordHash = passwordHash;
			user.Created = DateTime.Now;
			user.Changed = DateTime.Now;
			var result = await _userManager.CreateAsync(user);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, "CORELdap");
			}
			return await _userManager.FindByNameAsync(user.UserName);
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
		public async Task<JsonResult> LocalLogout()
		{
			try
			{
				await _signInManager.SignOutAsync();
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				return Json(new { message = "Error on logout!!", success = false });
			}

			return Json(new { message = "Log Out success", success = true });
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
			var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
			var result = await _userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				_logger.LogInformation("User created a new account with password.");

				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
				await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

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
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string code = null)
		{
			if (code == null) throw new ApplicationException("A code must be supplied for password reset.");
			var model = new ResetPasswordViewModel { Code = code };
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null) return RedirectToAction(nameof(ResetPasswordConfirmation));
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded) return RedirectToAction(nameof(ResetPasswordConfirmation));
			this.AddIdentityErrors(result);
			return View();
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
			var user = await _userManager.GetUserAsync(User);
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
				default:
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
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		#region Helpers

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		#endregion Helpers
	}
}