using System;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Identity.PhoneVerification.Abstractions;
using GR.Identity.PhoneVerification.Abstractions.Helpers;
using Microsoft.AspNetCore.Identity;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace GR.Identity.PhoneVerification.Infrastructure
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class SmsSenderService : ISmsSender
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject options
        /// </summary>
        private readonly IWritableOptions<TwilioSettings> _options;

        #endregion

        public SmsSenderService(IUserManager<GearUser> userManager, IWritableOptions<TwilioSettings> options)
        {
            _userManager = userManager;
            _options = options;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendAsync(string subject, string message, string to)
        {
            var result = new ResultModel();
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            if (user == null || !user.PhoneNumberConfirmed) return new UserNotFoundResult<object>().ToBase();
            var phoneResponse = PhoneVerificationUtil.Parse(to);
            if (!phoneResponse.IsSuccess) return phoneResponse.ToBase();
            try
            {
                TwilioClient.Init(_options.Value.AccountSid, _options.Value.AuthToken);
                var sendResponse = MessageResource.Create(
                    body: message,
                    from: new PhoneNumber(_options.Value.PhoneNumber),
                    to: new PhoneNumber(to)
                );
                if (sendResponse.Status == MessageResource.StatusEnum.Sent || _options.Value.IsTestEnv)
                {
                    result.IsSuccess = true;
                    return result;
                }

                result.AddError(sendResponse.ErrorCode?.ToString(), sendResponse.ErrorMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result.AddError(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Send sms
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="user"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResultModel> SendAsync<TUser>(TUser user, string subject, string message) where TUser : IdentityUser<Guid>
        {
            return await SendAsync(subject, message, user.PhoneNumber);
        }

        /// <summary>
        /// Get provider
        /// </summary>
        /// <returns></returns>
        public virtual object GetProvider() => this;
    }
}