using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Identity.Abstractions;
using GR.Identity.Mpass.Abstractions;
using GR.Identity.Mpass.Abstractions.Helpers;
using GR.Identity.Mpass.Abstractions.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GR.Identity.Mpass.Razor.Controllers
{
    public class MpassAccountController : Controller
    {

        /// <summary>
        /// Return url
        /// </summary>
        private const string ReturnUrl = "ReturnUrl";

        private const string MpassRequestSessionKey = "mpass_request_id";
        private const string SamlRequest = "SAMLRequest";


        #region Injectable

        /// <summary>
        /// Inject Mpass options
        /// </summary>
        private readonly IOptions<MPassOptions> _mpassOptions;
        /// <summary>
        /// Inject M pass service
        /// </summary>
        private readonly IMPassService _mpassService;

        /// <summary>
        /// Inject mpass store
        /// </summary>
        private readonly IMPassSigningCredentialsStore _mpassSigningCredentialStore;

        /// <summary>
        /// Inject cache
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<MpassAccountController> _logger;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        #endregion

        public MpassAccountController(IOptions<MPassOptions> mpassOptions, IMPassSigningCredentialsStore mpassSigningCredentialStore, IMPassService mpassService, IDistributedCache cache, ILogger<MpassAccountController> logger, IUserManager<GearUser> userManager, SignInManager<GearUser> signInManager)
        {
            _mpassOptions = mpassOptions;
            _mpassSigningCredentialStore = mpassSigningCredentialStore;
            _mpassService = mpassService;
            _cache = cache;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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
                $"{GearApplication.SystemConfig.EntryUri}/Account/Acs", TimeSpan.FromDays(30D),
                usrInfo.RequestId, _mpassOptions.Value.SAMLIssuer, out var ns, out var sessionIndex, out var nameID,
                out var attributes);

            var existingUser = await _userManager.UserManager.FindByNameAsync(usrInfo.NameID);

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
