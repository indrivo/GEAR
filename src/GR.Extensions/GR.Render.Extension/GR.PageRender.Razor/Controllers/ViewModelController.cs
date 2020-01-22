using GR.Core;
using GR.Core.Attributes;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GR.PageRender.Razor.Controllers
{
    [Authorize]
    public class ViewModelController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject view model service
        /// </summary>
        private readonly IViewModelService _viewModelService;

        /// <summary>
        /// Inject page render
        /// </summary>
        private readonly IPageRender _pageRender;

        #endregion Injectable

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelController(IViewModelService viewModelService, IPageRender pageRender)
        {
            _viewModelService = viewModelService;
            _pageRender = pageRender;
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var viewModelRequest = await _viewModelService.GetViewModelByIdAsync(id);
            if (!viewModelRequest.IsSuccess) return NotFound();
            return View(viewModelRequest.Result);
        }

        /// <summary>
        /// Edit template of view model field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TemplateEdit([Required] Guid id)
        {
            var viewModelFieldRequest = await _viewModelService.GetViewModelFieldByIdAsync(id);
            if (!viewModelFieldRequest.IsSuccess) return NotFound();
            return View(viewModelFieldRequest.Result);
        }

        /// <summary>
        /// Template edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> TemplateEdit(ViewModelFields model)
        {
            var updateRequest = await _viewModelService.UpdateViewModelFieldTemplateAsync(model);
            if (updateRequest.IsSuccess) return RedirectToAction("OrderFields", new { Id = updateRequest.Result });
            ModelState.AppendResultModelErrors(updateRequest.Errors);
            return View(model);
        }

        /// <summary>
        /// Update view model name
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit([Required] ViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var saveResult = await _viewModelService.UpdateViewModelAsync(model);
            if (saveResult.IsSuccess) return RedirectToAction("Index");
            ModelState.AppendResultModelErrors(saveResult.Errors);
            return View(model);
        }

        /// <summary>
        /// Create view model
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GenerateViewModel([Required] Guid entityId)
        {
            return Json(await _pageRender.GenerateViewModel(entityId));
        }

        /// <summary>
        /// Update styles of page
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateViewModelFields([Required] IEnumerable<ViewModelFields> items)
            => await JsonAsync(_viewModelService.UpdateItemsAsync(items.ToList()));

        /// <summary>
        /// Order fields
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OrderFields([Required] Guid id)
        {
            if (Guid.Empty == id) return NotFound();
            var viewModel = await _viewModelService.PagesContext.ViewModels
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.TableModelFields)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (viewModel == null) return NotFound();
            ViewBag.ViewModel = viewModel;
            var model = viewModel.ViewModelFields
                .OrderBy(x => x.Order).ToList();
            return View(model);
        }

        /// <summary>
        /// Load page types with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadViewModels(DTParameters param, Guid entityId)
            => Json(_viewModelService.LoadViewModelsWithPagination(param, entityId));

        /// <summary>
        /// Delete page type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Delete(Guid? id)
            => await JsonAsync(_viewModelService.RemoveViewModelAsync(id));

        /// <summary>
        /// Fields mapping
        /// </summary>
        /// <param name="viewModelId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FieldsMapping([Required] Guid viewModelId)
        {
            var viewModel = await _viewModelService.PagesContext.ViewModels
                .Include(x => x.TableModel)
                .ThenInclude(x => x.TableFields)
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.TableModelFields)
                .FirstOrDefaultAsync(x => x.Id == viewModelId);
            if (viewModel == null) return NotFound();

            var baseProps = BaseModel.GetPropsName();
            var tableFields = viewModel.TableModel.TableFields.Where(x => !baseProps.Contains(x.Name)).ToList();
            ViewBag.ViewModel = viewModel;
            ViewBag.TableFields = tableFields;
            ViewBag.BaseProps = baseProps;
            return View();
        }

        /// <summary>
        /// Save new mapping
        /// </summary>
        /// <param name="viewModelFieldId"></param>
        /// <param name="tableFieldId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> FieldsMapping([Required] Guid viewModelFieldId, Guid tableFieldId)
            => await JsonAsync(_viewModelService.SetFieldsMappingAsync(viewModelFieldId, tableFieldId));

        /// <summary>
        /// Update translated text for view model field
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="translatedKey"></param>
        /// <returns></returns>
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        [Route("api/[controller]/[action]")]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> ChangeViewModelFieldTranslateText([Required] Guid fieldId,
            [Required] string translatedKey)
            => await JsonAsync(_viewModelService.ChangeViewModelFieldTranslateTextAsync(fieldId, translatedKey));

        /// <summary>
        /// Set many to many configurations
        /// </summary>
        /// <param name="referenceEntity"></param>
        /// <param name="storageEntity"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public virtual async Task<JsonResult> SaveManyToManyConfigurations(Guid? referenceEntity, Guid? storageEntity, Guid? fieldId)
            => await JsonAsync(_viewModelService.SaveManyToManyConfigurationsAsync(referenceEntity, storageEntity, fieldId));
    }
}