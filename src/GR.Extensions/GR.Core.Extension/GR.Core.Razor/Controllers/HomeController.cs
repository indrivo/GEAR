using System.Diagnostics;
using System.Linq;
using GR.Core.Helpers;
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
        /// Inject provider of routes
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
        public IActionResult Index()
        {
            if (AppRoutes.RegisteredRoutes.Any()) return View();
            var routes = _provider.ActionDescriptors.Items.Select(x =>
                Url.Action(x.RouteValues["Action"], x.RouteValues["Controller"]).ToLowerInvariant()).ToList();
            AppRoutes.RegisteredRoutes = routes;
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