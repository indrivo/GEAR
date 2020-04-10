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
    }
}