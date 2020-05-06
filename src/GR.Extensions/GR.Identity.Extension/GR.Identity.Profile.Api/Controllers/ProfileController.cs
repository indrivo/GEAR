using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Profile.Api.Controllers
{
    [JsonApiExceptionFilter]
    [GearAuthorize(GearAuthenticationScheme.Bearer | GearAuthenticationScheme.Identity)]
    [Route("api/[controller]/[action]")]
    public class ProfileController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject profile service
        /// </summary>
        private readonly IProfileService _profileService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="profileService"></param>
        /// <param name="userManager"></param>
        public ProfileController(IProfileService profileService, IUserManager<GearUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        /// <summary>
        /// Update user profile info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> EditProfile(UserProfileEditViewModel model)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            return await JsonAsync(_profileService.UpdateBaseUserProfileAsync(model));
        }

        /// <summary>
        /// Upload uer photo
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> UploadUserPhoto([Required]IFormFile file)
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
            resultModel.AppendIdentityErrors(result.Errors);
            return Json(resultModel);
        }

        /// <summary>
        /// Remove user photo
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveUserPhoto()
            => await JsonAsync(_userManager.RemoveUserPhotoAsync());


        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> UserPasswordChange([Required]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            return await JsonAsync(_userManager.ChangeUserPasswordAsync(model.CurrentPassword, model.Password));
        }
    }
}
