using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Audit.Abstractions;
using GR.Audit.Abstractions.Helpers;
using GR.Audit.Abstractions.ViewModels.AuditViewModels;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Audit.Razor.Controllers
{
    [Authorize]
    public class AuditController : Controller
    {
        /// <summary>
        /// Inject audit manager
        /// </summary>
        private readonly IAuditManager _auditManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="auditManager"></param>
        public AuditController(IAuditManager auditManager)
        {
            _auditManager = auditManager;
        }

        /// <summary>
		/// Get list of audit Core
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Modules
        /// </summary>
        /// <returns></returns>
        public IActionResult Modules()
        {
            var modules = TrackerContextsInMemory.GetAll().Select(x => x.Key);
            return View(modules);
        }

        /// <summary>
        /// Module audit
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public IActionResult ModuleAudit(string module)
        {
            if (module.IsNullOrEmpty()) return NotFound();
            ViewBag.Module = module;
            return View();
        }

        /// <summary>
		/// For ajax request
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[AjaxOnly]
        [HttpPost]
        public JsonResult TrackAuditList(DTParameters param)
        {
            var filtered = _auditManager.GetAllFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount).ToList();

            var finalResult = new DTResult<TrackAuditsListViewModel>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(finalResult);
        }

        /// <summary>
        /// For ajax request
        /// </summary>
        /// <param name="param"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [AjaxOnly]
        [HttpPost]
        public JsonResult TrackAuditModuleList(DTParameters param, string moduleName)
        {
            var filtered = _auditManager.GetAllForModuleFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount, moduleName).ToList();

            var finalResult = new DTResult<TrackAuditsListViewModel>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(finalResult);
        }

        /// <summary>
        /// Get details audit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id, string moduleName)
        {
            if (id == null || string.IsNullOrEmpty(moduleName))
            {
                return NotFound();
            }

            var track = await _auditManager.GetDetailsAsync(id, moduleName);

            if (!track.IsSuccess) return NotFound();

            return View(track.Result);
        }

        /// <summary>
        /// Get versions audit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Versions(Guid? id, string moduleName)
        {
            if (id == null || string.IsNullOrEmpty(moduleName)) return NotFound();

            var reqVersions = await _auditManager.GetVersionsAsync(id, moduleName);
            if (!reqVersions.IsSuccess) return NotFound();
            return View(reqVersions.Result);
        }
    }
}