using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Users.Razor.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/Users/[action]")]
    public class UsersApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        public UsersApiController(IUserManager<GearUser> userManager)
        {
            _userManager = userManager;
        }


        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<UserInfoViewModel>))]
        public async Task<JsonResult> GetUserById([Required] Guid? userId)
        {
            if (userId == null) return Json(new NotFoundResultModel());
            var user = await _userManager.IdentityContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            return Json(new ResultModel<UserInfoViewModel>
            {
                IsSuccess = true,
                Result = user.Adapt<UserInfoViewModel>()
            });
        }

        /// <summary>
        /// Get current user info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<UserInfoViewModel>))]
        public async Task<JsonResult> GetCurrentUserInfo()
        {
            var currentUserReq = await _userManager.GetCurrentUserAsync();
            return !currentUserReq.IsSuccess
                ? Json(currentUserReq)
                : Json(currentUserReq.Map(currentUserReq.Result.Adapt<UserInfoViewModel>()));
        }

        /// <summary>
        /// Disable user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DisableUser()
        {
            var userIdReq = _userManager.FindUserIdInClaims();
            if (!userIdReq.IsSuccess) return Json(userIdReq);
            var disableReq = await _userManager.DisableUserAsync(userIdReq.Result);
            return Json(disableReq);
        }

        /// <summary>
        /// Enable user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> EnableUser()
        {
            var userIdReq = _userManager.FindUserIdInClaims();
            if (!userIdReq.IsSuccess) return Json(userIdReq);
            var disableReq = await _userManager.EnableUserAsync(userIdReq.Result);
            return Json(disableReq);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteUser()
        {
            var userIdReq = _userManager.FindUserIdInClaims();
            if (!userIdReq.IsSuccess) return Json(userIdReq);
            var disableReq = await _userManager.DeleteUserAsync(userIdReq.Result);
            return Json(disableReq);
        }

        /// <summary>
        /// Restore user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RestoreUserByUserId([Required] Guid userId)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            var restoreReq = await _userManager.RestoreUserAsync(userId);
            return Json(restoreReq);
        }

        /// <summary>
        /// Restore by phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RestoreUserByPhoneNumber([Required, DataType(DataType.PhoneNumber)] string phoneNumber)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            var user = await _userManager.UserManager.Users
                .FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));
            if (user == null) return Json(new UserNotFoundResult<object>());
            if (!user.IsDisabled) return Json(new InvalidParametersResultModel("User is already active"));
            var restoreReq = await _userManager.RestoreUserAsync(user.Id);
            return Json(restoreReq);
        }

        /// <summary>
        /// Restore by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RestoreUserByEmail([Required, DataType(DataType.EmailAddress), EmailAddress] string email)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            var user = await _userManager.UserManager.FindByEmailAsync(email);
            if (user == null) return Json(new UserNotFoundResult<object>());
            if (!user.IsDisabled) return Json(new InvalidParametersResultModel("User is already active"));
            var restoreReq = await _userManager.RestoreUserAsync(user.Id);
            return Json(restoreReq);
        }

        /// <summary>
        /// Load user with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Admin]
        [JsonProduces(typeof(DTResult<UserListItemViewModel>))]
        public async Task<JsonResult> GetUsersWithPagination(DTParameters param)
        {
            var data = await _userManager.GetAllUsersWithPaginationAsync(param);
            return Json(data);
        }
    }
}