using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Extensions;
using GR.Documents.Abstractions.Helpers;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.Documents.Razor.Controllers
{
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
        public IActionResult Create(DocumentTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            var result = _documentTypeService.SaveDocumentTypeAsync(model);


            return View(model);
        }


        public JsonResult Delete(Guid id)
        {


            return Json("");
        }
    }
}
