using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Models.ViewModels;
using GR.Report.Abstractions;

namespace GR.Dashboard.Razor.Controllers
{
    public sealed class WidgetController : Controller
    {
        #region Injectable 

        /// <summary>
        /// Inject widget service
        /// </summary>
        private readonly IWidgetService _widgetService;

        /// <summary>
        /// Inject report context
        /// </summary>
        private readonly IReportContext _reportContext;
        #endregion

        public WidgetController(IWidgetService widgetService, IReportContext reportContext)
        {
            _widgetService = widgetService;
            _reportContext = reportContext;
        }
        /// <summary>
        /// Custom widgets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create new widget
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            var model = new WidgetViewModel
            {
                Groups = _widgetService.Groups.ToList()
            };
            return View(model);
        }

        /// <summary>
        /// Create new widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(WidgetViewModel model)
        {
            model.Groups = await _widgetService.Groups.ToListAsync();
            if (!ModelState.IsValid) return View(model);
            var result = await _widgetService.CreateWidgetAsync(model);
            if (result.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(result.Errors);
            return View(model);
        }


        /// <summary>
        /// Get edit mode view for widget group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (!id.HasValue) return NotFound();
            var widget = await _widgetService.Widgets.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (widget == null) return NotFound();
            var model = widget.Adapt<WidgetViewModel>();
            model.Groups = await _widgetService.Groups.ToListAsync();
            return View(model);
        }

        /// <summary>
        /// Commit changes to widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(WidgetViewModel model)
        {
            model.Groups = await _widgetService.Groups.ToListAsync();
            if (!ModelState.IsValid) return View(model);
            var dbUpdate = await _widgetService.UpdateWidgetAsync(model);
            if (dbUpdate.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(dbUpdate.Errors);
            return View(model);
        }

        /// <summary>
        /// Get jquery list
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCustomWidgets(DTParameters parameters) => _widgetService.GetWidgetsInJqueryTableFormat(parameters);

        /// <summary>
        /// Get reports
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetReports()
        {
            var reports = await _reportContext.DynamicReports.ToDictionaryAsync(x => x.Id, x => x.Name);
            return Json(reports);
        }

        /// <summary>
        /// Import report
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> ImportReportAsWidget(Guid? reportId)
        {
            var commit = await _widgetService.CreateNewReportWidgetAsync(reportId);
            return Json(commit);
        }
    }
}