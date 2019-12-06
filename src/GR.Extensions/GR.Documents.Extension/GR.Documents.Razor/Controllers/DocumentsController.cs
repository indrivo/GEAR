using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        private readonly IDocumentServiceWithWorkflow _documentServiceWithWorkflowice;
        private readonly IDocumentTypeService _documentTypeService;
        private IWorkFlowExecutorService _workFlowExecutorService;


        #endregion

        #region Helpers

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        #endregion

        public DocumentsController(IDocumentServiceWithWorkflow documentServiceWithWorkflowice, IDocumentTypeService documentTypeService, IWorkFlowExecutorService workFlowExecutorService)
        {
            _documentServiceWithWorkflowice = documentServiceWithWorkflowice;
            _documentTypeService = documentTypeService;
            _workFlowExecutorService = workFlowExecutorService;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {

            //var listDocuments = await _documentService.GetAllDocumentsAsync();
            //var listResult = listDocuments.Result.Adapt<IEnumerable<DocumentViewModel>>();

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDocuments()
        {
            var result = await _documentServiceWithWorkflowice.GetAllDocumentsAsync();

            var listDocument = result.Result;
            foreach (var document in listDocument)
            {
                var workFlow = (await _workFlowExecutorService.GetEntryStatesAsync(document.LastVersionId.ToString())).Result.FirstOrDefault()?.Contract?.WorkFlowId;
                if (workFlow != null)
                {
                    document.CurrentStateName = (await _workFlowExecutorService.GetEntryStateAsync(document.LastVersionId.ToString(), workFlow)).Result.State.Name;
                    document.ListNextState = (await _workFlowExecutorService.GetNextStatesForEntryAsync(document.LastVersionId.ToString(), workFlow)).Result.ToList();
                }

            }

            return Json(result, SerializerSettings);
        }


        [HttpGet]
        public async Task<JsonResult> GetDocumentsByIdAsync(Guid? documentId)
        {
            var toReturn = new ResultModel();

            if (documentId is null)
            {
                toReturn.IsSuccess = false;
                toReturn.Errors.Add(new ErrorModel { Message = "document Id not found" });
                return Json(toReturn, SerializerSettings);
            }

            var result = await _documentServiceWithWorkflowice.GetDocumentsByIdAsync(documentId);

            if (!result.IsSuccess)
                return Json(result, SerializerSettings);

            var document = new AddDocumentViewModel
            {
                DocumentId = result.Result.Id,
                Description = result.Result.Description,
                Group = result.Result.Group,
                DocumentCode = result.Result.DocumentCode,
                Tile = result.Result.Title,
                DocumentTypeId = result.Result.DocumentTypeId,
            };

            toReturn.Result = document;
            toReturn.IsSuccess = true;

            return Json(toReturn, SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllDocumentsByListId(List<Guid> listDocumentId)
        {
            var result = await _documentServiceWithWorkflowice.GetAllDocumentsByListId(listDocumentId);
            return Json(result, SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllDocumentsByTypeIdAndList(List<Guid> listDocumetId, Guid? typeId)
        {
            var result = await _documentServiceWithWorkflowice.GetAllDocumentsByTypeIdAndListAsync(typeId, listDocumetId);
            return Json(result, SerializerSettings);
        }


        [HttpPost]
        public async Task<JsonResult> DeleteDocumnetsByListIdAsync(List<Guid> listDocumetId)
        {
            var result = await _documentServiceWithWorkflowice.DeleteDocumentsByListIdAsync(listDocumetId);
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

            result = await _documentServiceWithWorkflowice.AddDocumentAsync(model);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> Edit(AddDocumentViewModel model)
        {
            var result = new ResultModel();

            if (!ModelState.IsValid)
            {
                result.IsSuccess = false;
                result.Result = model;
                return Json(result);
            }

            result = await _documentServiceWithWorkflowice.EditDocumentAsync(model);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDocumentVersion(Guid? documentId)
        {
            return Json(await _documentServiceWithWorkflowice.GetAllDocumentVersionByIdAsync(documentId), SerializerSettings);
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

            result = await _documentServiceWithWorkflowice.AddNewDocumentVersionAsync(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllDocumentByTypeAsync(Guid? typeId)
        {
            return Json(await _documentServiceWithWorkflowice.GetAllDocumentsByTypeIdAsync(typeId), SerializerSettings);
        }


        [HttpPost]
        public async Task<JsonResult> ChangeDocumentStatus(ChangeDocumentStatusViewModel model)
        {
            //(model.EntryId, model.WorkFlowId,model.NewStateId
            var result = await _workFlowExecutorService.ChangeStateForEntryAsync(new ObjectChangeStateViewModel
            {
                EntryId = model.EntryId,
                WorkFlowId = model.WorkFlowId,
                NewStateId = model.NewStateId
                //Message = ""
                //EntryObjectConfiguration = need document
            });

            if (!result.IsSuccess) return Json(result);

            var currentStateName = (await _workFlowExecutorService.GetEntryStateAsync(model.EntryId, model.WorkFlowId)).Result.State.Name;
            var listNextState = (await _workFlowExecutorService.GetNextStatesForEntryAsync(model.EntryId, model.WorkFlowId)).Result.ToList();

            var resultModel = new ResultModel
            {
                IsSuccess = currentStateName != null,
                Result = new { model.EntryId, currentStateName, listNextState }
            };
            return Json(resultModel);
        }
    }
}
