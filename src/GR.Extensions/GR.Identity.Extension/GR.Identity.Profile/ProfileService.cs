using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;

namespace GR.Identity.Profile
{
    public class ProfileService : IProfileService
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;


        /// <summary>
        /// Inject profile context
        /// </summary>
        private readonly IProfileContext _profileContext;

        #endregion

        public ProfileService(IProfileContext profileContext, IUserManager<GearUser> userManager)
        {
            _profileContext = profileContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Update base user profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateBaseUserProfileAsync(UserProfileEditViewModel model)
        {
            var resultModel = new ResultModel();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found!"));
                return resultModel;
            }

            var currentUser = currentUserRequest.Result;
            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.Birthday = model.Birthday;
            currentUser.AboutMe = model.AboutMe;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.EmailConfirmed = currentUser.EmailConfirmed && model.Email.Equals(currentUser.Email);
            currentUser.Email = model.Email;

            var result = await _userManager.UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return resultModel;
            }

            resultModel.AppendIdentityErrors(result.Errors);

            return resultModel;
        }
    }
}