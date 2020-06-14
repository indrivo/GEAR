using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.Email.Abstractions.Events;
using GR.Email.Abstractions.Events.EventArgs;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class ProfileService : IProfileService
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject http context accessor
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        #endregion

        public ProfileService(IUserManager<GearUser> userManager, IHttpContextAccessor accessor)
        {
            _userManager = userManager;
            _accessor = accessor;
        }

        /// <summary>
        /// Update base user profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateBaseUserProfileAsync(UserProfileEditViewModel model)
        {
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState;

            var resultModel = new ResultModel();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found!"));
                return resultModel;
            }

            var currentUser = currentUserRequest.Result;

            var isUsed = await _userManager.UserManager
                .Users
                .AsNoTracking()
                .AnyAsync(x => !x.Id.Equals(currentUser.Id)
                                     && !x.Email.IsNullOrEmpty()
                                     && x.Email.ToLowerInvariant()
                                         .Equals(model.Email.ToLowerInvariant()));

            if (isUsed)
            {
                resultModel.AddError("Email is used by another user");
                return resultModel;
            }

            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.Birthday = model.Birthday;
            currentUser.AboutMe = model.AboutMe;
            currentUser.PhoneNumber = model.PhoneNumber;
            currentUser.EmailConfirmed = currentUser.EmailConfirmed && model.Email.Equals(currentUser.Email);
            currentUser.Email = model.Email;

            if (!currentUser.EmailConfirmed)
            {
                EmailEvents.Events.TriggerSendConfirmEmail(new SendConfirmEmailEventArgs
                {
                    HttpContext = _accessor.HttpContext,
                    Email = model.Email
                });
            }

            var result = await _userManager.UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return resultModel;
            }

            resultModel.AppendIdentityErrors(result.Errors);

            return resultModel;
        }

        /// <summary>
        /// Change email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeEmailAsync(ChangeEmailViewModel model)
        {
            var modelState = ModelValidator.IsValid(model);
            if (!modelState.IsSuccess) return modelState;

            var resultModel = new ResultModel();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                resultModel.AddError("User not found!");
                return resultModel;
            }

            var currentUser = currentUserRequest.Result;

            var isUsed = await _userManager.UserManager
                .Users
                .AsNoTracking()
                .AnyAsync(x => !x.Id.Equals(currentUser.Id)
                               && !x.Email.IsNullOrEmpty()
                               && x.Email.ToLowerInvariant()
                                   .Equals(model.Email.ToLowerInvariant()));

            if (isUsed)
            {
                resultModel.AddError("Email is used by another user");
                return resultModel;
            }

            currentUser.EmailConfirmed = currentUser.EmailConfirmed && model.Email.Equals(currentUser.Email);
            currentUser.Email = model.Email;

            if (!currentUser.EmailConfirmed)
            {
                EmailEvents.Events.TriggerSendConfirmEmail(new SendConfirmEmailEventArgs
                {
                    HttpContext = _accessor.HttpContext,
                    Email = model.Email
                });
            }

            var result = await _userManager.UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return resultModel;
            }

            resultModel.AppendIdentityErrors(result.Errors);
            return resultModel;
        }

        /// <summary>
        /// Resend confirm email
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> ResendConfirmEmailAsync()
        {
            var resultModel = new ResultModel();
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found!"));
                return resultModel;
            }

            var currentUser = currentUserRequest.Result;

            if (!currentUser.EmailConfirmed)
            {
                EmailEvents.Events.TriggerSendConfirmEmail(new SendConfirmEmailEventArgs
                {
                    HttpContext = _accessor.HttpContext,
                    Email = currentUser.Email
                });
                resultModel.IsSuccess = true;
            }
            else
            {
                resultModel.AddError("Email has already been confirmed, try refreshing the page");
            }

            return resultModel;
        }
    }
}