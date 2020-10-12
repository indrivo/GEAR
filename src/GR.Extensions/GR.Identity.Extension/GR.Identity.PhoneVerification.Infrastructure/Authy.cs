using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Identity.PhoneVerification.Abstractions;
using GR.Identity.PhoneVerification.Abstractions.Events;
using GR.Identity.PhoneVerification.Abstractions.Events.EventArgs;
using GR.Identity.PhoneVerification.Abstractions.Helpers;
using GR.Identity.PhoneVerification.Abstractions.Helpers.Enums;
using GR.Identity.PhoneVerification.Abstractions.ViewModels;
using GR.Localization.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PhoneNumbers;

namespace GR.Identity.PhoneVerification.Infrastructure
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class Authy : IAuthy
    {
        #region Injectable

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<Authy> _logger;

        /// <summary>
        /// Inject http client
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Inject identity user manager
        /// </summary>
        private readonly UserManager<GearUser> _identityUserManager;

        /// <summary>
        /// Inject Twilio settings
        /// </summary>
        private readonly TwilioSettings _twilioSettings;

        /// <summary>
        /// Inject http context accessor
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Inject country service
        /// </summary>
        private readonly ICountryService _countryService;

        #endregion

        public Authy(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, UserManager<GearUser> identityUserManager, IWritableOptions<TwilioSettings> options, IHttpContextAccessor accessor, ICountryService countryService)
        {
            _identityUserManager = identityUserManager;
            _accessor = accessor;
            _countryService = countryService;
            _twilioSettings = options.Value;
            _logger = loggerFactory.CreateLogger<Authy>();

            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://api.authy.com");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("user-agent", GearApplication.ApplicationName);
            _client.DefaultRequestHeaders.Add("X-Authy-API-Key", _twilioSettings.AuthyApiKey);
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> RegisterUserAsync(RegisterViewModel user)
        {
            if (_twilioSettings.IsTestEnv) return new SuccessResultModel<string>(PhoneVerificationResources.TEST_ENV_TEXT);

            var userRegData = new Dictionary<string, string>
            {
                { "email", user.Email },
                { "country_code", user.CountryCode },
                { "cellphone", user.PhoneNumber }
            };

            var userRegRequestData = new Dictionary<string, object>
            {
                {"user", userRegData}
            };

            var result = await _client.PostJsonAsync("/protected/json/users/new", userRegRequestData);
            var strResponse = await result.Content.ReadAsStringAsync();
            _logger.LogDebug(strResponse);
            var obj = JObject.Parse(strResponse);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogWarning("Register user failed with Twilio authy service, response: {response}, payload: {data}", strResponse, userRegRequestData.SerializeAsJson());
                var apiError = obj.SelectToken("errors")?.SelectToken("message")?.Value<string>() ?? "Unknown error";
                return new ApiNotRespondResultModel<string>(apiError);
            }

            var isSuccess = obj.SelectToken("success")?.Value<bool>() ?? false;
            if (isSuccess)
            {
                var userId = obj.SelectToken("user")?.SelectToken("id")?.Value<string>() ?? string.Empty;
                return new SuccessResultModel<string>(userId);
            }

            _logger.LogWarning("Twilio fail to register user on authy service, response: {Response}", strResponse);

            var error = obj.SelectToken("errors")?.SelectToken("message")?.Value<string>() ?? string.Empty;

            return new ResultModel<string>()
                .AddError(error);
        }

        /// <summary>
        /// Verification of token
        /// </summary>
        /// <param name="authyId"></param>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> VerifyTokenAsync(string authyId, string token, Guid userId)
        {
            if (_twilioSettings.IsTestEnv)
            {
                if (token == PhoneVerificationResources.DEFAULT_CODE)
                    return new SuccessResultModel<string>(PhoneVerificationResources.TEST_ENV_TEXT);

                return new ResultModel<string>()
                    .AddError("Code not match")
                    .AddError(PhoneVerificationResources.TEST_ENV_TEXT);
            }

            var result = await _client.GetAsync($"/protected/json/verify/{token}/{authyId}");
            _logger.LogDebug(result.ToString());
            if (!result.IsSuccessStatusCode) return new ApiNotRespondResultModel<string>();
            var clientResponse = await result.Content.ReadAsStringAsync();
            _logger.LogDebug(clientResponse);
            var obj = JObject.Parse(clientResponse);
            var isSuccess = obj.SelectToken("success")?.Value<bool>() ?? false;
            var message = obj.SelectToken("message")?.Value<string>() ?? string.Empty;

            return new ResultModel<string>
            {
                IsSuccess = isSuccess,
                Result = message
            }.AddError(message);
        }

        /// <summary>
        /// Verify phone token
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="countryCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> VerifyPhoneTokenAsync(string phoneNumber, string countryCode, string token)
        {
            if (_twilioSettings.IsTestEnv)
            {
                if (token == PhoneVerificationResources.DEFAULT_CODE)
                {
                    PhoneVerificationEvents.Events.PhoneVerified(new PhoneConfirmedEventArgs
                    {
                        PhoneNumber = NormalizePhoneNumber(countryCode, phoneNumber),
                        HttpContext = _accessor.HttpContext
                    });

                    return new SuccessResultModel<string>(PhoneVerificationResources.TEST_ENV_TEXT);
                }

                return new ResultModel<string>()
                    .AddError("Code not match")
                    .AddError(PhoneVerificationResources.TEST_ENV_TEXT);
            }

            var result = await _client.GetAsync(
                $"/protected/json/phones/verification/check?phone_number={phoneNumber}&country_code={countryCode}&verification_code={token}"
            );

            var message = await result.Content.ReadAsStringAsync();
            var obj = JObject.Parse(message);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogWarning("Fail to verify code, response: {Response}", message);
                return new ResultModel<string>()
                    .AddError(obj.SelectToken("error_code")?.Value<string>(), obj.SelectToken("message")?.Value<string>());
            }

            var isSuccess = obj.SelectToken("success")?.Value<bool>() ?? false;
            if (isSuccess)
            {
                PhoneVerificationEvents.Events.PhoneVerified(new PhoneConfirmedEventArgs
                {
                    PhoneNumber = NormalizePhoneNumber(countryCode, phoneNumber),
                    HttpContext = _accessor.HttpContext
                });
                return new SuccessResultModel<string>(message);
            }

            _logger.LogWarning("Fail to verify phone with Twilio, payload: {data}", message);
            var errorsObject = message.Deserialize<Dictionary<string, object>>();
            return new ResultModel<string>()
                .AddError(errorsObject["error_code"].ToString(), errorsObject["message"].ToString());
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="authyId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> SendSmsAsync(string authyId)
        {
            if (_twilioSettings.IsTestEnv)
            {
                return new SuccessResultModel<string>(PhoneVerificationResources.TEST_ENV_TEXT);
            }

            var result = await _client.GetAsync($"/protected/json/sms/{authyId}?force=true");
            _logger.LogDebug(result.ToString());
            if (!result.IsSuccessStatusCode) return new ApiNotRespondResultModel<string>();
            var smsResponse = await result.Content.ReadAsStringAsync();
            var obj = JObject.Parse(smsResponse);
            var isSuccess = obj.SelectToken("success")?.Value<bool>() ?? false;
            var message = obj.SelectToken("message")?.Value<string>();
            var res = new ResultModel<string>
            {
                IsSuccess = isSuccess,
                Result = message
            };

            if (!isSuccess) res.AddError(message);
            return res;
        }

        /// <summary>
        /// Get user token for authy service
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> GetUserAuthyTokenAsync(GearUser user)
        {
            if (user == null) return new UserNotFoundResult<string>();
            var userToken = await _identityUserManager.GetAuthenticationTokenAsync(user, PhoneVerificationResources.LOGIN_PROVIDER_NAME, PhoneVerificationResources.AUTHY_TOKEN);
            if (userToken.IsNullOrEmpty()) return new ResultModel<string>()
                .AddError("User not registered on this service");

            return new SuccessResultModel<string>(userToken);
        }

        /// <summary>
        /// Phone verification request
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<object>> PhoneVerificationRequestAsync(string countryCode, string phoneNumber, VerificationMethod method = VerificationMethod.Sms)
        {
            if (_twilioSettings.IsTestEnv) return new SuccessResultModel<object>(new
            {
                Message = PhoneVerificationResources.TEST_ENV_TEXT
            });

            var response = new ResultModel<object>();
            var result = await _client.PostAsync(
                $"/protected/json/phones/verification/start?via={method.ToString().ToLower()}&country_code={countryCode}&phone_number={phoneNumber}",
                null
            );

            var content = await result.Content.ReadAsStringAsync();
            var obj = JObject.Parse(content);
            if (!result.IsSuccessStatusCode)
            {
                response.AddError(obj.SelectToken("error_code")?.Value<string>(), obj.SelectToken("message")?.Value<string>());
                _logger.LogError("Fail to send sms verification code, country: {Country}, phone: {Phone} response: {Response}", countryCode, phoneNumber, content);
                return response;
            }

            _logger.LogDebug(content);

            var isSuccess = obj.SelectToken("success")?.Value<bool>() ?? false;
            response.IsSuccess = isSuccess;

            response.Result = content.Deserialize<object>();
            return response;
        }

        public virtual async Task<string> CreateApprovalRequestAsync(string authyId)
        {
            var requestData = new Dictionary<string, string> {
                { "message", "OneTouch Approval Request" },
                { "details", "My Message Details" },
                { "seconds_to_expire", "300" }
            };

            var result = await _client.PostJsonAsync(
                $"/onetouch/json/users/{authyId}/approval_requests",
                requestData
            );

            _logger.LogDebug(result.ToString());
            var strContent = await result.Content.ReadAsStringAsync();
            _logger.LogDebug(strContent);



            var content = await result.Content.ReadAsJsonAsync<Dictionary<string, object>>();
            var approvalRequestData = (JObject)content["approval_request"];

            return (string)approvalRequestData["uuid"];
        }

        public virtual async Task<object> CheckRequestStatusAsync(string oneTouchUuid)
        {
            var result = await _client.GetAsync($"/onetouch/json/approval_requests/{oneTouchUuid}");
            _logger.LogDebug(result.ToString());
            var strContent = await result.Content.ReadAsStringAsync();
            _logger.LogDebug(strContent);

            return await result.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Normalize phone number
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual string NormalizePhoneNumber(string countryCode, string phone)
        {
            return $"+{countryCode}{phone}";
        }

        /// <summary>
        /// Generate hidden phone
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual string GenerateHiddenPhoneNumber(string phone)
        {
            if (phone.IsNullOrEmpty()) return string.Empty;
            if (phone.Length < 3) return string.Empty;
            var x = new string('x', phone.Length - 3);
            var last = phone.Substring(phone.Length - 2);
            return $"+{x}{last}";
        }

        /// <summary>
        /// Validate phone
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<PhoneNumber>> ValidatePhoneAsync(string countryCode, string phone)
        {
            var response = new ResultModel<PhoneNumber>();
            var normalizedPhone = NormalizePhoneNumber(countryCode, phone);

            var countryRequestByCallingCode = await _countryService.GetCountriesInfoByCallingCodeAsync(countryCode);
            if (!countryRequestByCallingCode.IsSuccess || !countryRequestByCallingCode.Result.Any())
            {
                response.AddError("Country code invalid");
                return response;
            }

            var validPhone = false;
            var message = string.Empty;
            var region = "";
            PhoneNumber phoneNumber = null;
            foreach (var country in countryRequestByCallingCode.Result.ToList())
            {
                var checkPhone = PhoneVerificationUtil.Parse(normalizedPhone, country.Alpha2Code);
                if (checkPhone.IsSuccess)
                {
                    var isValid = PhoneVerificationUtil.PhoneHelper.IsValidNumber(checkPhone.Result);
                    if (isValid)
                    {
                        validPhone = true;
                        phoneNumber = checkPhone.Result;
                        region = country.Alpha2Code;
                        break;
                    }

                    var examplePhone = PhoneVerificationUtil.PhoneHelper.GetExampleNumber(country.Alpha2Code);
                    message = $"Invalid phone for country code, example of phone: {examplePhone?.NationalNumber}";
                }
                else
                {
                    message = checkPhone.Errors.FirstOrDefault()?.Message;
                }
            }

            if (!validPhone)
            {
                response.AddError(message);
                return response;
            }

            var validatedPhone = NormalizePhoneNumber(phoneNumber.CountryCode.ToString(), phoneNumber.NationalNumber.ToString());
            if (!validatedPhone.Equals(normalizedPhone))
            {
                var examplePhone = PhoneVerificationUtil.PhoneHelper.GetExampleNumber(region);
                response.AddError($"The format on the phone is not normalized, delete spaces or other symbols, eg: {examplePhone.NationalNumber}");
                return response;
            }

            response.IsSuccess = true;
            response.Result = phoneNumber;
            return response;
        }

        /// <summary>
        /// Check if phone is used by another user
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<ResultModel> CheckIfPhoneIsUsedByAnotherUserAsync(PhoneNumber number)
        {
            if (number == null) return new InvalidParametersResultModel();
            var response = new ResultModel();
            var possibleSameUsers = await _identityUserManager.Users
                .Where(x => x.PhoneNumber.EndsWith(number.NationalNumber.ToString())
                            || x.UserName.EndsWith(number.NationalNumber.ToString()))
                .ToListAsync();

            if (!possibleSameUsers.Any()) return new SuccessResultModel<object>().ToBase();

            var isUsedPhone = possibleSameUsers.Select(user => PhoneVerificationUtil.PhoneHelper
                    .IsNumberMatch(number, user.PhoneNumber))
                .Any(match => match == PhoneNumberUtil.MatchType.EXACT_MATCH);

            var isUsedUserName = possibleSameUsers.Select(user => PhoneVerificationUtil.PhoneHelper
                    .IsNumberMatch(number, user.UserName))
                .Any(match => match == PhoneNumberUtil.MatchType.EXACT_MATCH);

            var isSame = isUsedUserName || isUsedPhone;
            response.IsSuccess = !isSame;
            if (isSame) response.AddError("The number is already used by another user");
            return response;
        }
    }
}
