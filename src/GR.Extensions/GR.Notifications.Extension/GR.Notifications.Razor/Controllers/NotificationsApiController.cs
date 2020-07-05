using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Notifications.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GR.Notifications.Razor.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Route("api/notifications/[action]")]
    public class NotificationsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;

        #endregion

        protected override JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = GearSettings.Date.DateFormatWithTime
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notify"></param>
        public NotificationsApiController(INotify<GearRole> notify)
        {
            _notify = notify;
        }

        /// <summary>
        /// Get notifications by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="onlyUnread"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = DefaultCacheResponseSeconds, Location = ResponseCacheLocation.Any, NoStore = false)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<SystemNotifications>>))]
        public async Task<JsonResult> GetNotificationsByUserId([Required] Guid userId, bool onlyUnread = true)
            => await JsonAsync(_notify.GetNotificationsByUserIdAsync(userId, onlyUnread));

        /// <summary>
        /// Get notifications with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="onlyUnread"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = DefaultCacheResponseSeconds, Location = ResponseCacheLocation.Any, NoStore = false)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<PaginatedNotificationsViewModel>))]
        public async Task<JsonResult> GetUserNotificationsWithPagination([Required] uint page = 1, uint perPage = 10, bool onlyUnread = true)
            => await JsonAsync(_notify.GetUserNotificationsWithPaginationAsync(page, perPage, onlyUnread));

        /// <summary>
        /// Get notification by id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = DefaultCacheResponseSeconds, Location = ResponseCacheLocation.Any, NoStore = false)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Dictionary<string, object>>))]
        public async Task<JsonResult> GetNotificationById(Guid? notificationId)
            => await JsonAsync(_notify.GetNotificationByIdAsync(notificationId));

        /// <summary>
        /// Mark notification as read
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> MarkAsRead([Required] Guid notificationId)
            => await JsonAsync(_notify.MarkAsReadAsync(notificationId));

        /// <summary>
        /// Permanent delete
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> PermanentlyDeleteNotification([Required] Guid? notificationId)
            => await JsonAsync(_notify.PermanentlyDeleteNotificationAsync(notificationId));

        /// <summary>
        /// Clear all
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> ClearAllByUserId([Required] Guid userId)
            => await JsonAsync(_notify.ClearAllUserNotificationsAsync(userId));
    }
}