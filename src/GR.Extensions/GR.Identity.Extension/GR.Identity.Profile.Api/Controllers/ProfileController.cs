using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ProfileController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject profile service
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        /// Inject profile context
        /// </summary>
        private readonly IProfileContext _profileContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;
        #endregion

        public ProfileController(IProfileService profileService, IUserManager<GearUser> userManager, IProfileContext profileContext)
        {
            _profileService = profileService;
            _userManager = userManager;
            _profileContext = profileContext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> AddUserProfileAddress(AddUserProfileAddressViewModel model)
        {
            var resultModel = new ResultModel();

            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid model"));
                return Json(resultModel);
            }

            var currentUser = (await _userManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found"));
                return Json(resultModel);
            }

            var address = new Address
            {
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                Created = DateTime.Now,
                ContactName = model.ContactName,
                ZipCode = model.ZipCode,
                Phone = model.Phone,
                CountryId = model.CountryId,
                StateOrProvinceId = model.CityId,
                User = currentUser,
                IsDefault = model.IsDefault
            };

            if (model.IsDefault)
            {
                _profileContext.UserAddresses
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            await _profileContext.UserAddresses.AddAsync(address);
            var result = await _profileContext.PushAsync();
            if (!result.IsSuccess)
            {
                foreach (var resultError in result.Errors)
                {
                    resultModel.Errors.Add(new ErrorModel(resultError.Key, resultError.Message));
                }

                return Json(resultModel);
            }

            resultModel.IsSuccess = true;
            return Json(resultModel);
        }

        /// <summary>
        /// Update user profile info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> EditProfile(UserProfileEditViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid model"));
                return Json(resultModel);
            }

            var currentUser = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (currentUser == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found!"));
                return Json(resultModel);
            }

            currentUser.FirstName = model.FirstName;
            currentUser.LastName = model.LastName;
            currentUser.Birthday = model.Birthday;
            currentUser.AboutMe = model.AboutMe;
            currentUser.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return Json(resultModel);
            }

            foreach (var identityError in result.Errors)
            {
                resultModel.Errors.Add(new ErrorModel(identityError.Code, identityError.Description));
            }

            return Json(resultModel);
        }

        [HttpPost]
        public virtual async Task<JsonResult> UploadUserPhoto(IFormFile file)
        {
            var resultModel = new ResultModel();
            if (file == null || file.Length == 0)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Image not found" });
                return Json(resultModel);
            }

            var currentUser = (await _userManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "User not found" });
                return Json(resultModel);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                currentUser.UserPhoto = memoryStream.ToArray();
            }

            var result = await _userManager.UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return Json(resultModel);
            }

            resultModel.IsSuccess = false;
            foreach (var error in result.Errors)
            {
                resultModel.Errors.Add(new ErrorModel { Key = error.Code, Message = error.Description });
            }

            return Json(resultModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> UserPasswordChange(ChangePasswordViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Invalid model" });
                return Json(resultModel);
            }

            var currentUser = (await _userManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "User not found" });
                return Json(resultModel);
            }

            var result = await _userManager.UserManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.Password);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                IdentityEvents.Users.UserPasswordChange(new UserChangePasswordEventArgs
                {
                    Email = currentUser.Email,
                    UserName = currentUser.UserName,
                    UserId = currentUser.Id,
                    Password = model.Password
                });
                return Json(resultModel);
            }

            resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Error on change password" });
            return Json(resultModel);
        }


        [HttpPost]
        public virtual async Task<JsonResult> DeleteUserAddress(Guid? id)
        {
            var resultModel = new ResultModel();
            if (!id.HasValue)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Null id"));
                return Json(resultModel);
            }

            var currentAddress = await _profileContext.UserAddresses.FindAsync(id.Value);
            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return Json(resultModel);
            }

            currentAddress.IsDeleted = true;
            var result = await _profileContext.PushAsync();
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    resultModel.Errors.Add(new ErrorModel(error.Key, error.Message));
                }

                return Json(resultModel);
            }

            resultModel.IsSuccess = true;
            return Json(resultModel);
        }
    }
}
