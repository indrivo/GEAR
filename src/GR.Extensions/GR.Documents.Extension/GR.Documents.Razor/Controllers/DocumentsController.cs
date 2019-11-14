using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.Documents.Razor.Controllers
{
    public class DocumentsController : Controller
    {

        #region Injectable

        private IDocumentService _documentService;
        private IDocumentTypeService _documentTypeService;

        #endregion


        public DocumentsController(IDocumentService documentService, IDocumentTypeService documentTypeService)
        {
            _documentService = documentService;
            _documentTypeService = documentTypeService;
        }


        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {

            var listDocuments = await _documentService.GetAllDocumentsAsync();
            var listResult = listDocuments.Result.Adapt<IEnumerable<DocumentViewModel>>();

            return View(listResult);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var documentAddModel = new AddDocumentViewModel();
            documentAddModel.ListDocumentTypes = (await _documentTypeService.GetAllDocumentTypeAsync()).Result.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            return View(documentAddModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(AddDocumentViewModel model)
        {

            if(!ModelState.IsValid)
                return View(model);

            var result = await _documentService.AddDocumentAsync(model);

            return RedirectToAction("Index");
        }
    }
}
