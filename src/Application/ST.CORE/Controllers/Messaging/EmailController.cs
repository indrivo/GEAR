using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.BaseBusinessRepository;
using ST.CORE.Extensions;
using ST.CORE.Models.NotificationsViewModels;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Identity.Data.UserProfiles;
using ST.Notifications.Abstraction;
using ST.Notifications.Hubs;
using ST.Notifications.Services;

namespace ST.CORE.Controllers.Messaging
{
	public class EmailController : Controller
	{
		private UserManager<ApplicationUser> UserManager { get; }
		
		private ILogger<EmailController> Logger { get; }

		private INotificationHub NotificationHub { get; }

		private readonly IHubContext<NotificationsHub> _hubContext;

		private static Notificator Notificator1 { get; } = IoC.Resolve<Notificator>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="logger"></param>
		/// <param name="notificationHub"></param>
		public EmailController(
			UserManager<ApplicationUser> userManager,
			ILogger<EmailController> logger, INotificationHub notificationHub, IHubContext<NotificationsHub> hubContext)
		{
			UserManager = userManager;
			Logger = logger;
			NotificationHub = notificationHub;
			_hubContext = hubContext;
		}

		/// <summary>
		/// Get Notifications page
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index()
		{
			await _hubContext.Clients.All.SendCoreAsync(SignalrSendMethods.SendClientNotification, new []{"test"});
			return View();
		}

		/// <summary>
		/// Get view for send notification
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SendNotification()
		{
			var users = await UserManager.Users.ToListAsync();
			ViewData["users"] = users;
			return View();
		}

		/// <summary>
		/// Send notification
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> SendNotification(NotificationCreateViewModel model)
		{
			var user = await UserManager.GetUserAsync(HttpContext.User);
			var notificationModel = new EntityViewModel
			{
				Fields = new List<EntityFieldsViewModel>
				{
					new EntityFieldsViewModel {ColumnName = "Author"},
					new EntityFieldsViewModel {ColumnName = "Subject"},
					new EntityFieldsViewModel {ColumnName = "Message"},
					new EntityFieldsViewModel {ColumnName = "Recipients"},
				},
				Values = new List<Dictionary<string, object>>
				{
					new Dictionary<string, object>
					{
						{"Author", user.Id},
						{"Subject", model.Subject},
						{"Message", model.Message},
						{"Recipients", model.Recipients}
					}
				}
			};

			var webNotification = new SignalrEmail
			{
				Subject = model.Subject,
				Message = model.Message,
				EmailRecipients = model.Recipients.ToList(),
				UserId = Guid.Parse(user.Id)
			};
			NotificationHub.SentEmailNotification(webNotification);

			var response = Notificator1.Create(notificationModel);
			if (response.IsSuccess)
				return RedirectToAction("Index", "Email", new { page = 1, perPage = 10, mode = true });
			foreach (var error in response.Errors)
			{
				ModelState.AddModelError(error.Key, error.Message);
			}

			return View(model);
		}

		/// <summary>
		/// Get folders
		/// </summary>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpGet, Produces("application/json", Type = typeof(ResultModel))]
		public async Task<JsonResult> GetFolders()
		{
			var userCur = await UserManager.GetUserAsync(HttpContext.User);
			var folders = Notificator1.GetUserFolders(userCur.Id, true);
			return Json(folders);
		}

		/// <summary>
		/// Move notification to folder
		/// </summary>
		/// <param name="folderId"></param>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		public JsonResult MoveToFolder([Required] Guid folderId, [Required] Guid notificationId)
		{
			var response = Notificator1.ChangeFolder(new EntityViewModel
			{
				Values = new List<Dictionary<string, object>>
				{
					new Dictionary<string, object>
					{
						{"Id", notificationId},
						{"UserEmailFolderId", folderId}
					}
				}
			});
			return Json(response);
		}

		/// <summary>
		/// Mark as read notification
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		public JsonResult MarkAsRead([Required] Guid notificationId)
		{
			var response = Notificator1.MarkAsRead(new List<Guid> { notificationId });
			return Json(response);
		}

		/// <summary>
		/// Get list by folder id
		/// </summary>
		/// <param name="folderId"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpGet, Produces("application/json", Type = typeof(ResultModel))]
		public async Task<JsonResult> GetListByFolderId([Required] Guid folderId, [Required] int page)
		{
			var userCur = await UserManager.GetUserAsync(HttpContext.User);
			var response = Notificator1.ListNotificationsByFolder(new EntityViewModel
			{
				Fields = new List<EntityFieldsViewModel>
				{
					new EntityFieldsViewModel {ColumnName = "Message"},
					new EntityFieldsViewModel {ColumnName = "Subject"}
				},
				Values = new List<Dictionary<string, object>>
				{
					new Dictionary<string, object>
					{
						{"Author", userCur.Id},
						{"UserEmailFolderId", folderId}
					}
				}
			});
			var count = response.Result.Values.Count();
			response.Result.Values = response
				.Result
				.Values
				.Skip((page - 1) * 10)
				.Take(10)
				.ToList();

			return Json(new ResultModel
			{
				IsSuccess = response.IsSuccess,
				Result = new
				{
					Count = count,
					Notifications = response.Result
				}
			});
		}
		/// <summary>
		/// Get all unread notifications
		/// </summary>
		/// <param name="folderId"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpGet, Produces("application/json", Type = typeof(ResultModel))]
		public async Task<JsonResult> GetUnreadListByFolderId([Required] Guid folderId)
		{
			var userCur = await UserManager.GetUserAsync(HttpContext.User);
			var response = Notificator1.ListNotificationsByFolder(new EntityViewModel
			{
				Fields = new List<EntityFieldsViewModel>
				{
					new EntityFieldsViewModel {ColumnName = "Message"},
					new EntityFieldsViewModel {ColumnName = "Subject"}
				},
				Values = new List<Dictionary<string, object>>
				{
					new Dictionary<string, object>
					{
						{"Author", userCur.Id},
						{"UserEmailFolderId", folderId}
					}
				}
			});
			var count = response.Result.Values.Count();
			response.Result.Values = response
				.Result
				.Values.Where(x => !(bool)x["IsRead"]).ToList();

			return Json(new ResultModel
			{
				IsSuccess = response.IsSuccess,
				Result = new
				{
					Count = count,
					Notifications = response.Result
				}
			});
		}

		/// <summary>
		/// Get total notifications by folderid
		/// </summary>
		/// <param name="folderId"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpGet, Produces("application/json", Type = typeof(ResultModel))]
		public async Task<JsonResult> GetCountbyFolderId([Required] Guid folderId)
		{
			var count = 0;
			var userCur = await UserManager.GetUserAsync(HttpContext.User);
			var response = Notificator1.ListNotificationsByFolder(new EntityViewModel
			{
				Fields = new List<EntityFieldsViewModel>
				{
				},
				Values = new List<Dictionary<string, object>>
				{
					new Dictionary<string, object>
					{
						{"Author", userCur.Id},
						{"UserEmailFolderId", folderId}
					}
				}
			});
			if (response.IsSuccess)
			{
				count = response.Result.Values.Count();
			}

			return Json(new ResultModel
			{
				IsSuccess = response.IsSuccess,
				Result = count,
				Errors = response.Errors
			});
		}

		/// <summary>
		/// Restore notification from trash folder
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		public JsonResult RestoreFromTrash([Required] Guid notificationId)
		{
			var response = Notificator1.RestoreFromTrash(new List<Guid> { notificationId });
			return Json(response);
		}

		/// <summary>
		/// Get message by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IActionResult GetMessageById(Guid id)
		{
			ViewBag.NotificationId = id;
			var message = Notificator1.GetNotificationById(id);
			if (message.IsSuccess)
			{
				return View(message);
			}

			return NotFound();
		}
	}
}