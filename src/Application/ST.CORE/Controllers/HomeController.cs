using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.Audit.Extensions;
using ST.BaseBusinessRepository;
using ST.CORE.Models;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Notifications.Abstraction;

namespace ST.CORE.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		#region Inject

		private readonly IBaseBusinessRepository<ApplicationDbContext> _repository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly INotificationHub _hub;
		private readonly ApplicationDbContext _context;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="userManager"></param>
		/// <param name="hub"></param>
		public HomeController(IBaseBusinessRepository<ApplicationDbContext> repository,
			UserManager<ApplicationUser> userManager, INotificationHub hub, ApplicationDbContext context)
		{
			_repository = repository;
			_userManager = userManager;
			_hub = hub;
			_context = context;
		}

		/// <summary>
		/// Dashboard view
		/// </summary>
		/// <returns></returns>
		[Authorize]
		public async Task<IActionResult> Index()
		{
			var list = _context.GetTrackedEntities();
			ViewBag.TotalUsers = _hub.GetOnlineUsers().Count();
			ViewBag.TotalSessions = _hub.GetSessionsCount();
			//ViewBag.User = await _userManager.GetUserAsync(User);
			return View("Index");
		}

		public IActionResult Error() =>
			View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}