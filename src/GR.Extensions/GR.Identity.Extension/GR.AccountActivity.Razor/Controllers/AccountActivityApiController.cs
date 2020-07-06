using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.AccountActivity.Abstractions;
using GR.AccountActivity.Abstractions.ViewModels;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.AccountActivity.Razor.Controllers
{
    [Author(Authors.LUPEI_NICOLAE)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public class AccountActivityApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user activity service
        /// </summary>
        private readonly IUserActivityService _activityService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activityService"></param>
        /// <param name="userManager"></param>
        public AccountActivityApiController(IUserActivityService activityService, IUserManager<GearUser> userManager)
        {
            _activityService = activityService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get confirmed devices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> GetConfirmedDevices()
            => await JsonAsync(_activityService.GetConfirmedDevicesAsync());

        /// <summary>
        /// Send mail for confirm device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> SendConfirmNewDeviceMail([Required] Guid? deviceId)
            => await JsonAsync(_activityService.SendConfirmNewDeviceMailAsync(deviceId, HttpContext));

        /// <summary>
        /// Get confirmed devices with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<ConfirmedDevicesViewModel>))]
        public virtual async Task<JsonResult> GetConfirmedDevicesWithPagination(DTParameters parameters)
            => await JsonAsync(_activityService.GetPagedConfirmedDevicesAsync(parameters));

        /// <summary>
        /// Get user activity with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<UserActivityViewModel>))]
        public virtual async Task<JsonResult> GetUserActivityWithPagination(DTParameters parameters)
            => await JsonAsync(_activityService.GetPagedUserActivityAsync(parameters), DateFormatWithTimeSerializerSettings);


        /// <summary>
        /// Get user activity with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Admin]
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<UserActivityViewModel>))]
        public virtual async Task<JsonResult> GetActivitiesForUserWithPagination(DTParameters parameters, Guid userId)
            => await JsonAsync(_activityService.GetPagedUserActivityAsync(parameters, userId), DateFormatWithTimeSerializerSettings);

        /// <summary>
        /// Get web sessions
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<WebSessionViewModel>))]
        public virtual async Task<JsonResult> GetWebSessionsWithPagination(DTParameters parameters)
            => await JsonAsync(_activityService.GetWebSessionsAsync(parameters), DateFormatWithTimeSerializerSettings);

        /// <summary>
        /// Delete all other confirmed and non confirmed
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> DeleteOtherConfirmedDevices()
            => await JsonAsync(_activityService.DeleteOtherConfirmedDevicesAsync(HttpContext), DateFormatWithTimeSerializerSettings);

        /// <summary>
        /// Delete user device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> DeleteDevice([Required] Guid deviceId)
            => await JsonAsync(_activityService.DeleteUserDeviceAsync(deviceId));

        /// <summary>
        /// Sign out all user devices
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> SignOutAllWebUserDevicesAsync()
        {
            var userId = _userManager.FindUserIdInClaims().Result;
            return await JsonAsync(_activityService.SignOutUserOnAllDevicesAsync(userId));
        }
    }
}