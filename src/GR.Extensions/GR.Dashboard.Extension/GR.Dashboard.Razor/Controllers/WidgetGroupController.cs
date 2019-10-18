using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Models;

namespace GR.Dashboard.Razor.Controllers
{
    [Author("Lupei Nicolae", 1.1)]
    [Documentation("This controller serve for manage widget groups records")]
    public sealed class WidgetGroupController : Controller
    {
        #region Injectable 

        /// <summary>
        /// Inject repository
        /// </summary>
        private readonly IWidgetGroupRepository _repository;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository"></param>
        public WidgetGroupController(IWidgetGroupRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Widget group list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Create new 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create widget group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(WidgetGroup model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _repository.CreateWidgetGroupAsync(model);
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
            var dashboard = await _repository.WidgetGroups.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (dashboard == null) return NotFound();
            return View(dashboard);
        }

        /// <summary>
        /// Commit changes to dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(WidgetGroup model)
        {
            if (!ModelState.IsValid) return View(model);
            var dbUpdate = await _repository.UpdateWidgetGroupAsync(model);
            if (dbUpdate.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(dbUpdate.Errors);
            return View(model);
        }

        /// <summary>
        /// Get widget groups
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]"), Produces("application/json", Type = typeof(DTResult<WidgetGroup>))]
        public JsonResult GetWidgetGroups(DTParameters parameters) => _repository.GetWidgetGroupsInJqueryTableFormat(parameters);
    }
}