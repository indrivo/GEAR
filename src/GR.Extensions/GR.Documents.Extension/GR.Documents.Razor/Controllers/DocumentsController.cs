using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.Documents.Razor.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {

        #region Injectable

        private IDocumentService _documentService;
        private IDocumentTypeService _documentTypeService;

        #endregion

        #region Helpers

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

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
        public async Task<JsonResult> GetAllDocuments()
        {
            var result =   await _documentService.GetAllDocumentsAsync();
            return Json(result, SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllDocumentsByListId(List<Guid> listDocumetId)
        {
            var result = await _documentService.GetAllDocumentsByListId(listDocumetId);
            return Json(result, SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllDocumentsByTypeIdAndList(List<Guid> listDocumetId, Guid? typeId)
        {
            var result = await _documentService.GetAllDocumentsByTypeIdAndListAsync(typeId, listDocumetId);
            return Json(result, SerializerSettings);
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
        public async Task<JsonResult> Create(AddDocumentViewModel model)
        {
            var result = new ResultModel();

            if (!ModelState.IsValid)
            {
                result.IsSuccess = false;
                result.Result = model;

                return Json(result);
            }

            result = await _documentService.AddDocumentAsync(model);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDocumentVersion(Guid? documentId)
        {
            return Json(await _documentService.GetAllDocumentVersionByIdAsync(documentId), SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> AddNewDocumentVersion(AddNewVersionDocumentViewModel model)
        {
            var result = new ResultModel();

            //if (!ModelState.IsValid)
            //{
            //    result.IsSuccess = false;
            //    result.Result = model;
            //    result.Errors.Add(new ErrorModel { Message = "model is not valid" });
            //    return Json(result);
            //}

            result = await _documentService.AddNewDocumentVersionAsync(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDocumentByTypeAsync(Guid? typeId)
        {
            return Json(await _documentService.GetAllDocumentsByTypeIdAsync(typeId), SerializerSettings);
        }
    }
}
