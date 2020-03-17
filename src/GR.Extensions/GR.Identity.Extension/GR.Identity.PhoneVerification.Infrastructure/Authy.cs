using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.PhoneVerification.Abstractions;
using GR.Identity.PhoneVerification.Abstractions.Helpers.Enums;
using GR.Identity.PhoneVerification.Abstractions.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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

        #endregion

        public Authy(IConfiguration config, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Authy>();

            _client = clientFactory.CreateClient();
            _client.BaseAddress = new Uri("https://api.authy.com");
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("user-agent", "Twilio Account Security C# Sample");

            // Get Authy API Key from Configuration
            _client.DefaultRequestHeaders.Add("X-Authy-API-Key", config["TwilioSettings:AuthyApiKey"]);
        }


        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> RegisterUserAsync(RegisterViewModel user)
        {
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

            _logger.LogDebug(result.Content.ReadAsStringAsync().Result);

            var response = await result.Content.ReadAsJsonAsync<Dictionary<string, object>>();
            var obj = JObject.FromObject(response["user"])["id"].ToString();
            return new SuccessResultModel<string>(obj);
        }

        public virtual async Task<ResultModel<string>> VerifyTokenAsync(string authyId, string token)
        {
            var result = await _client.GetAsync($"/protected/json/verify/{token}/{authyId}");

            _logger.LogDebug(result.ToString());
            _logger.LogDebug(result.Content.ReadAsStringAsync().Result);

            var message = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return new SuccessResultModel<string>(message);
            }

            return new ResultModel<string>
            {
                Errors = new List<IErrorModel>
                {
                    new ErrorModel(string.Empty, message)
                }
            };
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
            var result = await _client.GetAsync(
                $"/protected/json/phones/verification/check?phone_number={phoneNumber}&country_code={countryCode}&verification_code={token}"
            );

            _logger.LogDebug(result.ToString());
            var message = await result.Content.ReadAsStringAsync();
            _logger.LogDebug(message);

            if (result.IsSuccessStatusCode) return new SuccessResultModel<string>(message);
            var errorsObject = message.Deserialize<Dictionary<string, object>>();
            return new ResultModel<string>
            {
                Errors = new List<IErrorModel>
                {
                    new ErrorModel(errorsObject["error_code"].ToString(), errorsObject["message"].ToString())
                }
            };
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="authyId"></param>
        /// <returns></returns>
        public virtual async Task<string> SendSmsAsync(string authyId)
        {
            var result = await _client.GetAsync($"/protected/json/sms/{authyId}?force=true");

            _logger.LogDebug(result.ToString());



            return await result.Content.ReadAsStringAsync();
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
            var response = new ResultModel<object>();
            var result = await _client.PostAsync(
                $"/protected/json/phones/verification/start?via={method.ToString().ToLower()}&country_code={countryCode}&phone_number={phoneNumber}",
                null
            );

            var content = await result.Content.ReadAsStringAsync();

            _logger.LogDebug(result.ToString());
            _logger.LogDebug(content);

            response.IsSuccess = result.IsSuccessStatusCode;
            response.Result = content.Deserialize<object>();
            return response;
        }

        public virtual async Task<string> CreateApprovalRequestAsync(string authyId)
        {
            var requestData = new Dictionary<string, string>() {
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
    }
}
