using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;

namespace GR.Identity.Profile.Abstractions
{
    public interface IProfileService
    {
        /// <summary>
        /// Update base user profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateBaseUserProfileAsync(UserProfileEditViewModel model);

        /// <summary>
        /// Change email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeEmailAsync(ChangeEmailViewModel model);

        /// <summary>
        /// Resend mail for confirm email 
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> ResendConfirmEmailAsync();
    }
}