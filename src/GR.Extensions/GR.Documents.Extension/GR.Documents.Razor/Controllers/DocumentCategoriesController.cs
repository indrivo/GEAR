using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Extensions;
using GR.Documents.Abstractions.Helpers;
using GR.Documents.Abstractions.ViewModels.DocumentCategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GR.Documents.Razor.Controllers
{
    [Authorize]
    public class DocumentCategoriesController : BaseGearController
    {
        #region Injectable

        private IDocumentCategoryService _documentCategoryService;

        #endregion

        public DocumentCategoriesController(IDocumentCategoryService documentCategoryService)
        {
            _documentCategoryService = documentCategoryService;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get all Document categories from table model
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ListDocumentCategoriesWithPagination(DTParameters param)
        {
            var list = await _documentCategoryService.GetAllDocumentCategoryAsync(param);
            return Json(list);
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Get all Document categories from table model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(DefaultApiRouteTemplate)]
        [JsonProduces(typeof(ResultModel<DocumentCategoryViewModel>))]
        public async Task<JsonResult> ListDocumentCategories()
        {
            var list = await _documentCategoryService.GetAllDocumentCategoryAsync();

            return Json(list);
        }

        /// <summary>
        /// Create document type 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(DocumentCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddError(ErrorKeys.InvalidModel);
                return View(model);
            }

            var result = await _documentCategoryService.SaveDocumentCategoryAsync(model);
            if (result.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(result.Errors);
            return View(model);
        }

        /// <summary>
        /// Get edit document type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            return View((await _documentCategoryService.GetDocumentCategoryByIdAsync(id)).Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DocumentCategoryViewModel model)
        {
            var result = await _documentCategoryService.EditDocumentCategoryAsync(model);

            return RedirectToAction("Edit", new { id = result.Result.Id });
        }


        /// <summary>
        /// Delete document type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(Guid? id)
        {
            await _documentCategoryService.DeleteDocumentCategoryAsync(id);

            return RedirectToAction("Index");
        }
    }
}
