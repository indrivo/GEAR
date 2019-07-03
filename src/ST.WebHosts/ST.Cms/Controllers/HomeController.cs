using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Notifications.Abstractions;
using ST.Cms.ViewModels.ViewModels;

namespace ST.Cms.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		#region Inject

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly INotificationHub _hub;
		private readonly ApplicationDbContext _context;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="hub"></param>
		/// <param name="context"></param>
		/// <param name="service"></param>
		public HomeController(UserManager<ApplicationUser> userManager, INotificationHub hub, ApplicationDbContext context)
		{
			_userManager = userManager;
			_hub = hub;
			_context = context;
		}

		/// <summary>
		/// Dashboard view
		/// </summary>
		/// <returns></returns>
		[Authorize]
		public IActionResult Index()
		{
			ViewBag.TotalUsers = _hub.GetOnlineUsers().Count();
			ViewBag.TotalSessions = _hub.GetSessionsCount();
			return View("Index");
		}

		public IActionResult Error() =>
			View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}