using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.Permissions;
using GR.Dashboard.Abstractions.Models.ViewModels;

namespace GR.Dashboard.Razor.Controllers
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
        public virtual async Task<JsonResult> OrderedList(DTParameters param) => Json(await _dashboardService.GetDashboards(param));

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


        /// <summary>
        /// Update dashboard configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel<IEnumerable<DashboardRowViewModel>>))]
        public async Task<IActionResult> SaveDashboardConfiguration([Required]DashBoardConfigurationViewModel configuration)
        {
            var commit = await _dashboardService.AddOrUpdateDashboardConfigurationAsync(configuration);
            return Json(commit);
        }

        /// <summary>
        /// Get dashboard rows
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel<IEnumerable<DashboardRowViewModel>>))]
        public async Task<JsonResult> GetDashboardRows(Guid? dashboardId)
        {
            var commit = await _dashboardService.GetDashBoardConfigurationAsync(dashboardId);
            return Json(commit);
        }

        /// <summary>
        /// Get dashboard rows
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        [HttpDelete, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteRow(Guid? rowId)
        {
            var commit = await _dashboardService.DeleteRowAsync(rowId);
            return Json(commit);
        }

        /// <summary>
        /// Delete mapped widget to row
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        [HttpDelete, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteMappedWidget(Guid? rowId, Guid? widgetId)
        {
            var commit = await _dashboardService.DeleteMappedWidgetToRowAsync(rowId, widgetId);
            return Json(commit);
        }

        /// <summary>
        /// Get widget acl
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel<IEnumerable<RowWidgetAcl>>))]
        public async Task<JsonResult> GetWidgetConf(Guid? widgetId, Guid? rowId)
        {
            return Json(await _dashboardService.GetRowWidgetAclInfoAsync(widgetId, rowId));
        }

        /// <summary>
        /// Update acl configuration
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <param name="conf"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateAclForWidget(Guid? widgetId, Guid? rowId, IEnumerable<RowWidgetAclBase> conf)
        {
            return Json(await _dashboardService.UpdateAclAsync(widgetId, rowId, conf));
        }


        /// <summary>
        /// Get widget acl
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel<WidgetUiSettings>))]
        public async Task<JsonResult> GetUISettingsForWidget(Guid? widgetId, Guid? rowId)
        {
            return Json(await _dashboardService.GetUISettingsForWidgetAsync(widgetId, rowId));
        }

        /// <summary>
        /// Update acl configuration
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <param name="uiSettings"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateUISettings(Guid? widgetId, Guid? rowId, WidgetUiSettings uiSettings)
        {
            return Json(await _dashboardService.UpdateUISettingsAsync(widgetId, rowId, uiSettings));
        }
    }
}