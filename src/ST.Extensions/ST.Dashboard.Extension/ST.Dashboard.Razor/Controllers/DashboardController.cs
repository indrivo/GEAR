using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Extensions;
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
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dashboardService"></param>
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
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
        /// View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Get edit mode view for dashboard
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? dashboardId)
        {
            if (!dashboardId.HasValue) return NotFound();
            var dashboard = await _dashboardService.DashBoards.FirstOrDefaultAsync(x => x.Id.Equals(dashboardId));
            if (dashboard == null) return NotFound();
            return View(dashboard);
        }

        /// <summary>
        /// Commit changes to dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(DashBoard model)
        {
            if (!ModelState.IsValid) return View(model);
            var dbUpdate = await _dashboardService.UpdateDashBoardAsync(model);
            if (dbUpdate.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(dbUpdate.Errors);
            return View(model);
        }

        /// <summary>
        /// Create dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(DashBoard model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _dashboardService.CreateDashBoardAsync(model);
            if (result.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(result.Errors);
            return View(model);
        }

        /// <summary>
        /// Ajax ordered list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult OrderedList(DTParameters param) => _dashboardService.GetDashboards(param);

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Builder(Guid? dashboardId)
        {
            if (!dashboardId.HasValue) return NotFound();
            var dashboard = await _dashboardService.DashBoards.FirstOrDefaultAsync(x => x.Id.Equals(dashboardId));
            if (dashboard == null) return NotFound();
            return View();
        }
    }
}