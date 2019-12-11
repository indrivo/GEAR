using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;

namespace GR.Cms.Controllers
{
	public class HandlerController : Controller
	{
		/// <summary>
		/// Inject configuration context
		/// </summary>
		private readonly ConfigurationDbContext _context;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		public HandlerController(ConfigurationDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Redirect to not found view
		/// </summary>
		/// <returns></returns>
		public new IActionResult NotFound()
		{
			return View();
		}

		/// <summary>
		/// Redirect to app
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public IActionResult RedirectToApp(string app)
		{
			if (string.IsNullOrEmpty(app)) return NotFound();
			var client = _context.Clients.FirstOrDefault(x => x.ClientId.Equals(app));
			return client != null ? Redirect(client.ClientUri) : NotFound();
		}

		/// <summary>
		/// Runtime logs
		/// </summary>
		/// <returns></returns>
		public IActionResult Logs()
		{
			return View();
		}
	}
}