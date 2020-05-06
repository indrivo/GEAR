using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.PhoneVerification.Abstractions.Helpers.Enums;
using GR.Identity.PhoneVerification.Abstractions.ViewModels;
using PhoneNumbers;

namespace GR.Identity.PhoneVerification.Abstractions
{
    public interface IAuthy
    {
        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel<string>> RegisterUserAsync(RegisterViewModel user);

        /// <summary>
        /// Verify token
        /// </summary>
        /// <param name="authyId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResultModel<string>> VerifyTokenAsync(string authyId, string token);

        /// <summary>
        /// Verify phone token
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="countryCode"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResultModel<string>> VerifyPhoneTokenAsync(string phoneNumber, string countryCode, string token);

        /// <summary>
        /// Send sms
        /// </summary>
        /// <param name="authyId"></param>
        /// <returns></returns>
        Task<ResultModel<string>> SendSmsAsync(string authyId);

        /// <summary>
        /// Phone verification via request
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        Task<ResultModel<object>> PhoneVerificationRequestAsync(string countryCode, string phoneNumber, VerificationMethod method = VerificationMethod.Sms);

        /// <summary>
        /// Create approval request
        /// </summary>
        /// <param name="authyId"></param>
        /// <returns></returns>
        Task<string> CreateApprovalRequestAsync(string authyId);

        /// <summary>
        /// Check request status 
        /// </summary>
        /// <param name="oneTouchUuid"></param>
        /// <returns></returns>
        Task<object> CheckRequestStatusAsync(string oneTouchUuid);

        /// <summary>
        /// Normalize phone number
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        string NormalizePhoneNumber(string countryCode, string phone);

        /// <summary>
        /// Get user auth token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel<string>> GetUserAuthyTokenAsync(GearUser user);

        /// <summary>
        /// Generate hidden number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        string GenerateHiddenPhoneNumber(string phone);

        /// <summary>
        /// Validate phone
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<ResultModel<PhoneNumber>> ValidatePhoneAsync(string countryCode, string phone);

        /// <summary>
        /// Check if phone is used by another user
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        Task<ResultModel> CheckIfPhoneIsUsedByAnotherUserAsync(PhoneNumber number);
    }
}