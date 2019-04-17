using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.Application.Razor.ViewModels;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Data;
using ST.Notifications.Abstractions;

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
		private readonly IDynamicService _service;

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="userManager"></param>
		/// <param name="hub"></param>
		/// <param name="context"></param>
		/// <param name="service"></param>
		public HomeController(IBaseBusinessRepository<ApplicationDbContext> repository,
			UserManager<ApplicationUser> userManager, INotificationHub hub, ApplicationDbContext context, IDynamicService service)
		{
			_repository = repository;
			_userManager = userManager;
			_hub = hub;
			_context = context;
			_service = service;
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