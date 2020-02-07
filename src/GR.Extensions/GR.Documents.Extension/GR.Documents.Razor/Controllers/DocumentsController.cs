using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
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
    public class DocumentsController : BaseGearController
    {

        #region Injectable

        private readonly IDocumentService _documentService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IWorkFlowExecutorService _workFlowExecutorService;
        private IDocumentCategoryService _documentCategoryService;


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
                            document.CurrentStateName = (await _workFlowExecutorService.GetEntryStateAsync(document.LastVersionId.ToString(),
                                    workFlow)).Result.State.Name;
                            document.ListNextState = (await _workFlowExecutorService.GetNextStatesForEntryAsync(
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
                return Json(new ResultModel
                {
                    IsSuccess = false,
                    Errors = new List<IErrorModel> { new ErrorModel { Message = "document Id not found" } }
                });
            }

            var result = await _documentService.GetDocumentsByIdAsync(documentId);
            return Json(result, SerializerSettings);
        }

        /// <summary>
        /// Get all document by list id
        /// </summary>
        /// <param name="listDocumentId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentViewModel>))]
        public async Task<JsonResult> GetAllDocumentsByListId(List<Guid> listDocumentId)
        {
            var result = await _documentService.GetAllDocumentsByListId(listDocumentId);

            if (!result.IsSuccess) return Json(result);

            var listDocument = result.Result.ToList();
            foreach (var document in listDocument.Where(document => document.DocumentCategory.Code == 1))
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

            result.Result = listDocument;

            return Json(result, SerializerSettings);
        }


        /// <summary>
        /// Get al documents by type and ignore documents by list id
        /// </summary>
        /// <param name="listDocumetId"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentViewModel>))]
        public async Task<JsonResult> GetAllDocumentsByCategoryIdAndList(List<Guid> listDocumentId, Guid? categoryId)
        {
            var result = await _documentService.GetAllDocumentsByCategoryIdAndListAsync(categoryId, listDocumentId);
            return Json(result, SerializerSettings);
        }


        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentViewModel>))]
        public async Task<JsonResult> DeleteDocumnentsByListIdAsync(List<Guid> listDocumentId)
        {
            var result = await _documentService.DeleteDocumentsByListIdAsync(listDocumentId);
            return Json(result, SerializerSettings);
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentViewModel>))]
        public async Task<JsonResult> DeleteDocumnentsByIdAsync(Guid documentId)
        {
            var listDocumentId = new List<Guid>();
            listDocumentId.Add(documentId);
            var result = await _documentService.DeleteDocumentsByListIdAsync(listDocumentId);
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


        /// <summary>
        /// Edit document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
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
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentVersionViewModel>))]
        public async Task<JsonResult> GetAllDocumentVersion(Guid? documentId)
        {
            var result = await _documentService.GetAllDocumentVersionByIdAsync(documentId);

            var listVersion = result.Result;

            foreach (var version in listVersion)
            {
                var workFlow = (await _workFlowExecutorService.GetEntryStatesAsync(version.Id.ToString())).Result
                    .FirstOrDefault()?.Contract?.WorkFlowId;

                version.CurrentStateName =
                    (await _workFlowExecutorService.GetEntryStateAsync(version.Id.ToString(),
                        workFlow)).Result.State.Name;
            }


            return Json(result, SerializerSettings);
        }

        /// <summary>
        /// Add new document version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
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


        /// <summary>
        /// Get all documents by type
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<DocumentViewModel>>))]
        public async Task<JsonResult> GetAllDocumentByTypeAsync(Guid? typeId)
        {
            return Json(await _documentService.GetAllDocumentsByTypeIdAsync(typeId), SerializerSettings);
        }

        /// <summary>
        /// Change document status
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> ChangeDocumentStatus(ChangeDocumentStatusViewModel model)
        {
            var documentRequest = await _documentService.GetDocumentByVersionId(model.EntryId.ToGuid());
            if (!documentRequest.IsSuccess) return Json(documentRequest);
            var document = documentRequest.Result;
            var result = await _workFlowExecutorService.ChangeStateForEntryAsync(new ObjectChangeStateViewModel
            {
                EntryId = model.EntryId,
                WorkFlowId = model.WorkFlowId,
                NewStateId = model.NewStateId,
                Message = model.Comments,
                EntryObjectConfiguration = new Dictionary<string, string>
                {
                    { "Name", document.Title }
                }
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


        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<DocumentStateViewModel>))]
        public async Task<JsonResult> GetDocumentStateAsync(Guid? versionId)
        {

            if (versionId == null)
                return Json(new ResultModel<DocumentStateViewModel> { IsSuccess = false, Errors = new List<IErrorModel> { new ErrorModel { Message = "Version id not found" } } });

            var resultModel = new ResultModel<DocumentStateViewModel>();
            var documentState = new DocumentStateViewModel();
            documentState.Id = versionId.Value;

            var workFlowId = (await _workFlowExecutorService.GetEntryStatesAsync(versionId.ToString())).Result
                .FirstOrDefault()?.Contract?.WorkFlowId;

            if (workFlowId != null)
            {
                var getCurrentState = await _workFlowExecutorService.GetEntryStateAsync(versionId.ToString(), workFlowId);
                var getNexListState = await _workFlowExecutorService.GetNextStatesForEntryAsync(versionId.ToString(), workFlowId);
                var getHistoryWorkflow = await _workFlowExecutorService.GetEntryHistoryByWorkflowIdAsync(workFlowId, versionId.ToString());

                if (getCurrentState.IsSuccess)
                    documentState.CurrentStateName = getCurrentState.Result.State.Name;

                if (getNexListState.IsSuccess)
                    documentState.ListNextState = getNexListState.Result.ToList();

                if (getNexListState.IsSuccess)
                    documentState.History = getHistoryWorkflow.Result.ToList();
            }

            resultModel.IsSuccess = true;
            resultModel.Result = documentState;

            return Json(resultModel, SerializerSettings);
        }
    }
}
