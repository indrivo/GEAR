using System.Diagnostics;
using GR.Core.Razor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Core.Razor.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
        /// <summary>
		/// Dashboard view
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
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