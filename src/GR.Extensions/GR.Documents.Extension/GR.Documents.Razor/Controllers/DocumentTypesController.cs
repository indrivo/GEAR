using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Extensions;
using GR.Documents.Abstractions.Helpers;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.Documents.Razor.Controllers
{
    [Authorize]
    public class DocumentTypesController : Controller
    {

        #region Injectable

        private IDocumentTypeService _documentTypeService;

        #endregion

        public DocumentTypesController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public  JsonResult ListDocumentTypes(DTParameters param)
        {
            var list =  _documentTypeService.GetAllDocumentType(param);
            return Json(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create document type 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(DocumentTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            var result = await _documentTypeService.SaveDocumentTypeAsync(model);


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
            return View((await _documentTypeService.GetDocumentTypeByIdAsync(id)).Result);
        }

        public async Task<IActionResult> Edit(DocumentTypeViewModel model)
        {
            var result = await _documentTypeService.EditDocumentTypeAsync(model);


            return RedirectToAction("Edit", new {id = result.Result.Id});

            //return View(result.Result);
        }


        /// <summary>
        /// Delete document type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(Guid? id)
        {
            var result = await _documentTypeService.DeleteDocumentTypeAsync(id);

           return  RedirectToAction("Index");
        }
    }
}
