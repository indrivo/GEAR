using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.PhoneVerification.Abstractions;
using GR.Identity.PhoneVerification.Abstractions.Helpers;
using GR.Identity.PhoneVerification.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.PhoneVerification.Api.Controllers
{
    [JsonApiExceptionFilter]
    [GearAuthorize(GearAuthenticationScheme.Bearer | GearAuthenticationScheme.Identity)]
    [Route("/api/[controller]/[action]"), Produces(ContentType.ApplicationJson)]
    public class ChangePhoneController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject authy service
        /// </summary>
        private readonly IAuthy _authy;

        #endregion

        public ChangePhoneController(IUserManager<GearUser> userManager, IAuthy authy)
        {
            _userManager = userManager;
            _authy = authy;
        }

        /// <summary>
        /// Send code 
        /// </summary>
        /// <param name="newPhone"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<object>))]
        public async Task<JsonResult> SendCode(ChangePhoneViewModel newPhone)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            var validPhone = await _authy.ValidatePhoneAsync(newPhone.CountryCode, newPhone.PhoneNumber);
            if (!validPhone.IsSuccess) return Json(validPhone.ToBase());
            var phone = _authy.NormalizePhoneNumber(newPhone.CountryCode, newPhone.PhoneNumber);
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            if (user.PhoneNumber.Equals(phone)) return Json(new ResultModel()
                .AddError("This is the currently used phone, try another number you own or cancel the phone change"));
            var isUsedPhoneCheck = await _authy.CheckIfPhoneIsUsedByAnotherUserAsync(validPhone.Result);
            if (!isUsedPhoneCheck.IsSuccess) return Json(isUsedPhoneCheck);

            HttpContext.Session.Set("phone_change_request", newPhone);
            var result = await _authy.PhoneVerificationRequestAsync(
                newPhone.CountryCode,
                newPhone.PhoneNumber
            );

            return Json(result);
        }

        /// <summary>
        /// Verify code
        /// </summary>
        /// <param name="tokenVerification"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> VerifyCode(TokenVerificationModel tokenVerification)
        {
            var result = new ResultModel();
            var verificationRequest = HttpContext.Session.Get<ChangePhoneViewModel>("phone_change_request");
            if (verificationRequest == null)
            {
                var response = new ResultModel();
                response.Errors.Add(new ErrorModel(string.Empty, "A request to send the token to the phone was not made"));
                response.Errors.Add(new ErrorModel(string.Empty, $"To start, call: {nameof(SendCode)}"));
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
            var addNewUserRequest = await _authy.RegisterUserAsync(new RegisterViewModel
            {
                CountryCode = verificationRequest.CountryCode,
                PhoneNumber = verificationRequest.PhoneNumber,
                UserName = verificationRequest.PhoneNumber
            });

            if (!addNewUserRequest.IsSuccess) return Json(addNewUserRequest);
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            var setTokenResult = await _userManager
                .UserManager
                .SetAuthenticationTokenAsync(user, PhoneVerificationResources.LOGIN_PROVIDER_NAME, PhoneVerificationResources.AUTHY_TOKEN,
                    addNewUserRequest.Result);
            if (!setTokenResult.Succeeded)
            {
                result.AppendIdentityErrors(setTokenResult.Errors);
                return Json(result);
            }

            user.PhoneNumber = phone;
            user.PhoneNumberConfirmed = true;
            var updateResult = await _userManager.UserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                result.AppendIdentityErrors(updateResult.Errors);
                return Json(result);
            }

            result.IsSuccess = true;
            return Json(result);
        }
    }
}