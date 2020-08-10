using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using GR.Core.Attributes.Validation;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
        public virtual async Task<JsonResult> UploadUserPhoto([Required][AllowedExtensions(new[]
        {
            ".png", ".jpeg", ".jpg", ".tiff", ".pjp", ".pjpeg", ".jfif",".tif", ".gif", ".svg", ".bmp"
        })] IFormFile file)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            using (var image = await Image.LoadAsync(file.OpenReadStream()))
            {
                if (image.Width <= 256 && image.Height <= 256) return await JsonAsync(_userManager.ChangeUserPhotoAsync(file));

                image.Mutate(x => x.Resize(256, 256));
                using (var ms = new MemoryStream())
                {
                    image.SaveAsPng(ms);
                    var newFile = new FormFile(ms, 0, ms.Length, file.Name, file.FileName)
                    {
                        Headers = file.Headers
                    };

                    return await JsonAsync(_userManager.ChangeUserPhotoAsync(newFile));
                }
            }
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
        public virtual async Task<JsonResult> UserPasswordChange([Required] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            return await JsonAsync(_userManager.ChangeUserPasswordAsync(model.CurrentPassword, model.Password));
        }

        /// <summary>
        /// Change email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> ChangeEmail([Required] ChangeEmailViewModel model)
            => await JsonAsync(_profileService.ChangeEmailAsync(model));

        /// <summary>
        /// Resend email for confirm email
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> ResendConfirmEmail()
            => await JsonAsync(_profileService.ResendConfirmEmailAsync());
    }
}