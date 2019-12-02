using System.Diagnostics;
using System.Linq;
using GR.Cms.ViewModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using GR.Notifications.Abstractions;

namespace GR.Cms.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		#region Inject

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly INotificationHub _hub;
		private readonly ApplicationDbContext _context;
		private readonly SignInManager<ApplicationUser> _signInManager;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="hub"></param>
		/// <param name="context"></param>
		/// <param name="signInManager"></param>
		public HomeController(UserManager<ApplicationUser> userManager, INotificationHub hub, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_hub = hub;
			_context = context;
			_signInManager = signInManager;
		}

		/// <summary>
		/// Dashboard view
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			ViewBag.TotalUsers = _hub.GetOnlineUsers().Count();
			ViewBag.TotalSessions = _hub.GetSessionsCount();
			return View();
		}
		public IActionResult Testplumber()
		{
			return View();
		}

		
		/// <summary>
		/// Error page
		/// </summary>
		/// <returns></returns>
		public IActionResult Error() =>
			View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}