using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Notifications.Abstractions;

namespace ST.Cms.Controllers.Notifications
{
	public class NotificationsController : Controller
	{
		/// <summary>
		/// Inject notifier
		/// </summary>
		private readonly INotify<ApplicationRole> _notify;
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="notify"></param>
		public NotificationsController(INotify<ApplicationRole> notify)
		{
			_notify = notify;
		}
		/// <summary>
		/// Get notifications by user id
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/[controller]/[action]")]
		public async Task<IActionResult> GetNotificationsByUserId([Required]Guid userId)
		{
			var notifications = await _notify.GetNotificationsByUserIdAsync(userId);
			return Json(notifications);
		}

		/// <summary>
		/// Get notification by id
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/[controller]/[action]")]
		public async Task<IActionResult> GetNotificationById(Guid? notificationId)
		{
			return notificationId == null ? Json(new ResultModel()) : Json(await _notify.GetNotificationById(notificationId.Value));
		}

		/// <summary>
		/// Mark notification as read
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/[controller]/[action]")]
		public async Task<IActionResult> MarkAsRead([Required] Guid notificationId)
		{
			var result = await _notify.MarkAsReadAsync(notificationId);
			return Json(result);
		}
		/// <summary>
		/// Clear all
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/[controller]/[action]")]
		public async Task<IActionResult> ClearAllByUserId([Required] Guid userId)
		{
			var result = new ResultModel
			{
				IsSuccess = true
			};
			var notifications = await _notify.GetNotificationsByUserIdAsync(userId);
			if (!notifications.IsSuccess) return Json(result);
			foreach (var notification in notifications.Result)
			{
				await _notify.MarkAsReadAsync(notification.Id);
			}

			return Json(result);
		}
	}
}