using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Extensions;
using GR.Documents.Abstractions.Helpers;
using GR.Documents.Abstractions.ViewModels.DocumentCategoryViewModels;
using Microsoft.AspNetCore.Mvc;


namespace GR.Documents.Razor.Controllers
{
    public class DocumentCategoriesController : Controller
    {

        #region Injectable

        private IDocumentCategoryService _documentCategoryService;

        #endregion

        public DocumentCategoriesController(IDocumentCategoryService documentCategoryService)
        {
            _documentCategoryService = documentCategoryService;
        }

        // GET: /<controller>/
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
        public JsonResult ListDocumentCategories(DTParameters param)
        {
            var list = _documentCategoryService.GetAllDocumentCategory(param);
            return Json(list);
        }

        public IActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// Get all Document categories from table model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentCategoryViewModel>))]
        public async Task<JsonResult> ListDocumentCategoriesAsync()
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
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            var result = await _documentCategoryService.SaveDocumentCategoryAsync(model);


            return View(result.Result);
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
