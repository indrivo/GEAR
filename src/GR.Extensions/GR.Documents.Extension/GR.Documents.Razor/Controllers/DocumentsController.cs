using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
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

        private readonly IDocumentService _documentService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IWorkFlowExecutorService _workFlowExecutorService;
        private IDocumentCategoryService _documentCategoryService;


        #endregion

        #region Helpers

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        #endregion

        public DocumentsController(IDocumentService documentService, IDocumentTypeService documentTypeService, IWorkFlowExecutorService workFlowExecutorService, IDocumentCategoryService documentCategoryService)
        {
            _documentService = documentService;
            _documentTypeService = documentTypeService;
            _workFlowExecutorService = workFlowExecutorService;
            _documentCategoryService = documentCategoryService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            ViewBag.ListDocumentType = (await _documentTypeService.GetAllDocumentTypeAsync()).Result.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
            }).ToList();
            ViewBag.ListDocumentCategory = (await _documentCategoryService.GetAllDocumentCategoryAsync()).Result.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString() + "_" + s.Code,
                
            }).ToList();

            return View();
        }


        /// <summary>
        /// Get all Document  from table model
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ListDocuments(DTParameters param)
        {
            var list = _documentService.GetAllDocument(param);

            var listDocument = list.Data;
            if (listDocument != null && listDocument.Any())
            {
                foreach (var document in listDocument)
                {
                    if (document.DocumentCategory.Code == 1)
                    {
                        var workFlow = (await _workFlowExecutorService.GetEntryStatesAsync(document.LastVersionId.ToString()))
                            .Result.FirstOrDefault()?.Contract?.WorkFlowId;
                        if (workFlow != null)
                        {
                            document.CurrentStateName =(await _workFlowExecutorService.GetEntryStateAsync(document.LastVersionId.ToString(),
                                    workFlow)).Result.State.Name;
                            document.ListNextState =(await _workFlowExecutorService.GetNextStatesForEntryAsync(
                                    document.LastVersionId.ToString(), workFlow)).Result.ToList();
                        }
                    }
                }
            }

            return Json(list, SerializerSettings);
        }

        /// <summary>
        /// get all documents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<DocumentViewModel>>))]
        public async Task<JsonResult> GetAllDocuments()
        {
            var result = await _documentService.GetAllDocumentsAsync();

            var listDocument = result.Result;
            foreach (var document in listDocument)
            {
                if (document.DocumentCategory.Code == 1)
                {
                    var workFlow = (await _workFlowExecutorService.GetEntryStatesAsync(document.LastVersionId.ToString())).Result
                        .FirstOrDefault()?.Contract?.WorkFlowId;
                    if (workFlow != null)
                    {
                        document.CurrentStateName =
                            (await _workFlowExecutorService.GetEntryStateAsync(document.LastVersionId.ToString(),
                                workFlow)).Result.State.Name;
                        document.ListNextState =
                            (await _workFlowExecutorService.GetNextStatesForEntryAsync(
                                document.LastVersionId.ToString(), workFlow)).Result.ToList();
                    }
                }

            }

            return Json(result, SerializerSettings);
        }


        /// <summary>
        /// Register entity contract
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentViewModel>))]
        public async Task<JsonResult> GetDocumentsByIdAsync(Guid? documentId)
        {
            if (documentId is null)
            {
                return Json(new ResultModel { 
                    IsSuccess = false,
                    Errors = new List<IErrorModel>{ new ErrorModel { Message = "document Id not found" }}
                });
            }

            var result = await _documentService.GetDocumentsByIdAsync(documentId);
            return Json(result, SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllDocumentsByListId(List<Guid> listDocumentId)
        {
            var result = await _documentService.GetAllDocumentsByListId(listDocumentId);
            return Json(result, SerializerSettings);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllDocumentsByTypeIdAndList(List<Guid> listDocumetId, Guid? typeId)
        {
            var result = await _documentService.GetAllDocumentsByTypeIdAndListAsync(typeId, listDocumetId);
            return Json(result, SerializerSettings);
        }


        [HttpPost]
        public async Task<JsonResult> DeleteDocumnetsByListIdAsync(List<Guid> listDocumetId)
        {
            var result = await _documentService.DeleteDocumentsByListIdAsync(listDocumetId);
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
            result = await _documentService.AddDocumentAsync(model);

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

            result = await _documentService.EditDocumentAsync(model);

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


        [HttpPost]
        public async Task<JsonResult> ChangeDocumentStatus(ChangeDocumentStatusViewModel model)
        {

            //var result = await _workFlowExecutorService.ChangeStateForEntryAsync(model.EntryId, model.WorkFlowId, model.NewStateId);

            //if (!result.IsSuccess)

            ////(model.EntryId, model.WorkFlowId,model.NewStateId
            var result = await _workFlowExecutorService.ChangeStateForEntryAsync(new ObjectChangeStateViewModel

            {
                EntryId = model.EntryId,
                WorkFlowId = model.WorkFlowId,
                NewStateId = model.NewStateId
                //Message = ""
                //EntryObjectConfiguration = need document
            });

            if (!result.IsSuccess) return Json(result);

            //var currentStateName = (await _workFlowExecutorService.GetEntryStateAsync(model.EntryId, model.WorkFlowId)).Result.State.Name;
            //var listNextState = (await _workFlowExecutorService.GetNextStatesForEntryAsync(model.EntryId, model.WorkFlowId)).Result.ToList();


            //var resultModel = new ResultModel();

            //resultModel.IsSuccess = currentStateName != null;
            //resultModel.Result = new { model.EntryId, currentStateName, listNextState };


            //var resultModel = new ResultModel
            //{
            //    IsSuccess = currentStateName != null,
            //    Result = new { model.EntryId, currentStateName, listNextState }
            //};

            return Json("");
        }
    }
}
