using GR.Core.Helpers;
using System.Threading.Tasks;
using GR.Identity.Abstractions;

namespace GR.TwoFactorAuthentication.Abstractions
{
    public interface ITwoFactorAuthService
    {
        /// <summary>
        /// Send code
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel<string>> SendCodeAsync(GearUser user);

        /// <summary>
        /// Validate received code
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ResultModel<string>> ValidateReceivedCodeAsync(GearUser user, string code);

        /// <summary>
        /// Get action message
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GetActionMessageAsync(GearUser user);
    }
}