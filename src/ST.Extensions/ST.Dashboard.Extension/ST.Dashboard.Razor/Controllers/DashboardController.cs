using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Abstractions;
using ST.Dashboard.Abstractions;
using ST.Dashboard.Abstractions.Models;

namespace ST.Dashboard.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class DashboardController : Controller
    {
        /// <summary>
        /// Inject dashboard manager
        /// </summary>
        private readonly IDashboardManager _dashboardManager;

        /// <summary>
        /// Inject data filter
        /// </summary>
        private readonly IDataFilter _dataFilter;

        public DashboardController(IDashboardManager dashboardManager, IDataFilter dataFilter)
        {
            _dashboardManager = dashboardManager;
            _dataFilter = dataFilter;
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Ajax ordered list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult OrderedList(DTParameters param) => _dashboardManager.GetDashboards(param);

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Builder()
        {
            return View();
        }
    }
}