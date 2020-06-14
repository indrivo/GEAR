using System.Diagnostics;
using System.Linq;
using GR.Core.Razor.Helpers;
using GR.Core.Razor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GR.Core.Razor.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject action descriptor service
        /// </summary>
        private readonly IActionDescriptorCollectionProvider _provider;

        #endregion

        public HomeController(IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Dashboard view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual IActionResult Index()
        {
            RegisterRoutes();
            return View();
        }

        /// <summary>
        /// Error page
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        /// <summary>
        /// Register routes
        /// </summary>
        protected void RegisterRoutes()
        {
            if (!AppRoutes.RegisteredRoutes.Any())
            {
                var routes = _provider.ActionDescriptors.Items.Select(x =>
                    Url.Action(x.RouteValues["Action"], x.RouteValues["Controller"]).ToLowerInvariant()).ToList();
                AppRoutes.RegisteredRoutes = routes;
            }
        }
    }
}