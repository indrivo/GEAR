using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.PhoneVerification.Abstractions;
using GR.Identity.PhoneVerification.Abstractions.Helpers.Enums;
using GR.Identity.PhoneVerification.Abstractions.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GR.Identity.PhoneVerification.Api.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.Bearer | GearAuthenticationScheme.Identity)]
    [Route("/api/[controller]/[action]"), Produces("application/json")]
    public sealed class PhoneAccountSecurityController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<PhoneAccountSecurityController> _logger;

        /// <summary>
        /// Inject authy service
        /// </summary>
        private readonly IAuthy _authy;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="logger"></param>
        /// <param name="authy"></param>
        public PhoneAccountSecurityController(IUserManager<GearUser> userManager, ILogger<PhoneAccountSecurityController> logger, IAuthy authy)
        {
            _userManager = userManager;
            _logger = logger;
            _authy = authy;
        }

        /// <summary>
        /// Start register via phone verification
        /// </summary>
        /// <param name="verificationRequest"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<object>))]
        public async Task<JsonResult> StartRegisterBySendingVerificationRequest(PhoneVerificationRequestModel verificationRequest)
        {
            var response = new ResultModel();
            if (!ModelState.IsValid) return JsonModelStateErrors();
            var userName = _authy.NormalizePhoneNumber(verificationRequest.CountryCode, verificationRequest.PhoneNumber);
            var user = await _userManager.UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "The user already exists"));
                return Json(response);
            }
            HttpContext.Session.Set("phone_verification_request", verificationRequest);
            var result = await _authy.PhoneVerificationRequestAsync(
                verificationRequest.CountryCode,
                verificationRequest.PhoneNumber,
                verificationRequest.Via
            );
            return Json(result);
        }

        /// <summary>
        /// Verify code of new registration
        /// </summary>
        /// <param name="tokenVerification"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<string>))]
        public async Task<JsonResult> VerifyAndCompletePhoneRegistration(TokenVerificationModel tokenVerification)
        {
            var verificationRequest = HttpContext.Session.Get<PhoneVerificationRequestModel>("phone_verification_request");
            if (verificationRequest == null)
            {
                var response = new ResultModel();
                response.Errors.Add(new ErrorModel(string.Empty, "A request to send the token to the phone was not made"));
                response.Errors.Add(new ErrorModel(string.Empty, $"To start, call: {nameof(StartRegisterBySendingVerificationRequest)}"));
                return Json(response);
            }

            if (!ModelState.IsValid) return JsonModelStateErrors();
            var validationResult = await _authy.VerifyPhoneTokenAsync(
                verificationRequest.PhoneNumber,
                verificationRequest.CountryCode,
                tokenVerification.Token
            );

            if (!validationResult.IsSuccess) return Json(validationResult);
            var phone = _authy.NormalizePhoneNumber(verificationRequest.CountryCode, verificationRequest.PhoneNumber);
            var createRequest = await _userManager.CreateUserAsync(new GearUser
            {
                PhoneNumber = phone,
                UserName = phone,
                IsEditable = true,
                PhoneNumberConfirmed = true
            }, verificationRequest.Pin);

            if (!createRequest.IsSuccess) return Json(createRequest);
            var addNewUserRequest = await _authy.RegisterUserAsync(new RegisterViewModel
            {
                CountryCode = verificationRequest.CountryCode,
                PhoneNumber = verificationRequest.PhoneNumber,
                Password = verificationRequest.Pin,
                UserName = verificationRequest.PhoneNumber
            });

            return Json(!addNewUserRequest.IsSuccess ? addNewUserRequest : validationResult);
        }


        [HttpPost]
        public async Task<JsonResult> Verify(TokenVerificationModel data)
        {
            var currentUser = await _userManager.UserManager.GetUserAsync(User);

            if (!ModelState.IsValid) return JsonModelStateErrors();
            ResultModel<string> result;

            if (data.Token.Length > 4)
            {
                result = await _authy.VerifyTokenAsync(currentUser.Id.ToString(), data.Token);
            }
            else
            {
                result = await _authy.VerifyPhoneTokenAsync(currentUser.PhoneNumber, "MD", data.Token);
            }

            _logger.LogDebug(result.ToString());

            if (!result.IsSuccess) return Json(result);
            await AddTokenVerificationClaim(currentUser);
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> Sms()
        {
            var currentUser = await _userManager.UserManager.GetUserAsync(User);
            var result = await _authy.SendSmsAsync(currentUser.Id.ToString());
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> Voice()
        {
            var currentUser = await _userManager.UserManager.GetUserAsync(User);
            var result = await _authy.PhoneVerificationRequestAsync(
                "MD",
                currentUser.PhoneNumber,
                VerificationMethod.Call
            );
            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> OneTouch()
        {
            var currentUser = await _userManager.UserManager.GetUserAsync(User);
            var oneTouchUuid = await _authy.CreateApprovalRequestAsync(currentUser.Id.ToString());
            HttpContext.Session.Set("onetouch_uuid", oneTouchUuid);
            return Json(oneTouchUuid);
        }

        [HttpPost]
        public async Task<JsonResult> OneTouchStatus()
        {
            var currentUser = await _userManager.UserManager.GetUserAsync(this.User);
            var oneTouchUuid = HttpContext.Session.Get<string>("onetouch_uuid");

            var result = await _authy.CheckRequestStatusAsync(oneTouchUuid);
            var data = (JObject)result;
            var approvalRequestStatus = (string)data["approval_request"]["status"];

            if (approvalRequestStatus != "approved") return Json(result);
            await AddTokenVerificationClaim(currentUser);
            await _userManager.UserManager.UpdateAsync(currentUser);

            return Json(result);
        }

        #region Helpers

        internal async Task<IList<Claim>> AddTokenVerificationClaim(GearUser user)
        {
            var tokenVerificationClaim = new Claim(ClaimTypes.AuthenticationMethod, "TokenVerification");
            var claims = new List<Claim>
            {
                tokenVerificationClaim
            };

            var userClaims = (List<Claim>)await _userManager.UserManager.GetClaimsAsync(user);

            if (userClaims.FindIndex(claim => claim.Value.Equals("TokenVerification")) == -1)
            {
                await _userManager.UserManager.AddClaimsAsync(user, claims);
            }

            return await _userManager.UserManager.GetClaimsAsync(user);
        }

        #endregion
    }
}