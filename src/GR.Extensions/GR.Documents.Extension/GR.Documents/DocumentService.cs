using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using GR.Files.Abstraction;
using GR.Files.Abstraction.Models.ViewModels;
using GR.Identity.Abstractions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.Documents
{
    [Author(Authors.DOROSENCO_ION, 1.1)]
    [Author(Authors.LUPEI_NICOLAE, 1.2, "Add virtual keyword for all methods and clean code")]
    public class DocumentService : IDocumentService
    {
        #region Injectable

        /// <summary>
        /// Inject db context 
        /// </summary>
        public readonly IDocumentContext _documentContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject file manager
        /// </summary>
        public readonly IFileManager FileManager;

        /// <summary>
        /// Inject data filter
        /// </summary>
        public readonly IDataFilter DataFilter;


        #endregion


        public DocumentService(IDocumentContext documentContext, IUserManager<GearUser> userManager, IFileManager fileManager, IDataFilter dataFilter)
        {
            _documentContext = documentContext;
            _userManager = userManager;
            FileManager = fileManager;
            DataFilter = dataFilter;
        }
        

        /// <summary>
        /// Get all document from table model
        /// </summary>
        /// <returns></returns>
        public virtual DTResult<DocumentViewModel> GetAllDocument(DTParameters param)
        {

            var filtered = DataFilter.FilterAbstractEntity<Document, IDocumentContext>(_documentContext, param.Search.Value,
                param.SortOrder,
                param.Start,
                param.Length,
                out var totalCount).Select(x =>
                {
                    x.DocumentVersions = _documentContext.DocumentVersions.Where(i=> i.DocumentId == x.Id).ToList();
                    x.DocumentType = _documentContext.DocumentTypes.FirstOrDefault(i => x.DocumentTypeId == i.Id) ?? new DocumentType();
                    x.DocumentCategory = _documentContext.DocumentCategories.FirstOrDefault(i => i.Id == x.DocumentCategoryId);
                    var listModel = x.Adapt<DocumentViewModel>();
                    return listModel;
                }).ToList();

            var result = new DTResult<DocumentViewModel>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return result;
        }

        /// <summary>
        /// Get all documents
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsAsync()
        {
            var user = await _userManager.GetCurrentUserAsync();
            var response = new ResultModel<IEnumerable<DocumentViewModel>>();

            var listDocuments = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i=> i.DocumentCategory)
                .Where(x => x.TenantId == user.Result.TenantId)
                .ToListAsync();

            if (listDocuments == null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();

            response.IsSuccess = true;
            response.Result = listDocuments.Adapt<List<DocumentViewModel>>();

            return response;
        }

        /// <summary>
        /// Get all document by type
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsByTypeIdAsync(Guid? typeId)
        {
            var result = new ResultModel<IEnumerable<DocumentViewModel>>();
            var user = await _userManager.GetCurrentUserAsync();

            if (typeId == null) return new InvalidParametersResultModel<IEnumerable<DocumentViewModel>>();

            var typeBd = await _documentContext.DocumentTypes.FirstOrDefaultAsync(x => x.Id == typeId);
            if (typeBd == null) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();


            var documentsBd = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i => i.DocumentCategory)
                .Where(x => x.DocumentTypeId == typeId && x.TenantId == user.Result.TenantId && !x.IsDeleted).ToListAsync();

            if (documentsBd == null || !documentsBd.Any()) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();

            result.IsSuccess = true;
            result.Result = documentsBd.Adapt<IEnumerable<DocumentViewModel>>();

            return result;
        }


        /// <summary>
        /// get documents by id an eliminate exist documents
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="listIgnoreDocuments"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsByCategoryIdAndListAsync(Guid? cetgoryId,
            IEnumerable<Guid> listIgnoreDocuments)
        {

            var result = new ResultModel<IEnumerable<DocumentViewModel>>();
            var user = await _userManager.GetCurrentUserAsync();

            if (cetgoryId == null) return new InvalidParametersResultModel<IEnumerable<DocumentViewModel>>();

            var typeBd = await _documentContext.DocumentCategories.FirstOrDefaultAsync(x => x.Id == cetgoryId);
            if (typeBd == null) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();

            var documentsBd = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i => i.DocumentCategory)
                .Where(x => x.DocumentCategoryId == cetgoryId && !listIgnoreDocuments.Contains(x.Id) && x.TenantId == user.Result.TenantId && !x.IsDeleted).ToListAsync();

            if (documentsBd == null || !documentsBd.Any()) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();

            result.IsSuccess = true;
            result.Result = documentsBd.Adapt<IEnumerable<DocumentViewModel>>();

            return result;

        }

        /// <summary>
        /// Get  documents by list id
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsByListId(IEnumerable<Guid> listDocumentId)
        {
            var response = new ResultModel<IEnumerable<DocumentViewModel>>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new ActionBlockedResultModel<IEnumerable<DocumentViewModel>>();
            var user = userRequest.Result;
            var enumeratedDocs = listDocumentId?.ToList();
            if (enumeratedDocs == null || !enumeratedDocs.Any()) return new InvalidParametersResultModel<IEnumerable<DocumentViewModel>>();

            var listDocuments = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i => i.DocumentCategory)
                .Where(x => enumeratedDocs.Contains(x.Id) && x.TenantId == user.TenantId && !x.IsDeleted)
                .ToListAsync();

            if (listDocuments == null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();

            response.IsSuccess = true;
            response.Result = listDocuments.Adapt<IEnumerable<DocumentViewModel>>();

            return response;
        }

        /// <summary>
        /// Delete documents by list id
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteDocumentsByListIdAsync(IEnumerable<Guid> listDocumentsId)
        {
            var response = new ResultModel();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new ActionBlockedResultModel<object>().ToBase();
            var user = userRequest.Result;
            var enumeratedDocs = listDocumentsId?.ToList();
            if (enumeratedDocs == null || !enumeratedDocs.Any())
            {
                response.Errors.Add(new ErrorModel { Message = "List documents == null" });
                return response;
            }

            var listDocuments = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i => i.DocumentCategory)
                .Where(x => enumeratedDocs.Contains(x.Id) && x.TenantId == user.TenantId)
                .ToListAsync();

            if (listDocuments == null || !listDocuments.Any())
            {
                response.Errors.Add(new ErrorModel { Message = "List documents  null" });
                return response;
            }

            foreach (var doc in listDocuments)
                doc.IsDeleted = true;


            _documentContext.Documents.UpdateRange(listDocuments);
            response = await _documentContext.PushAsync();

            return response;
        }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<DocumentViewModel>> GetDocumentsByIdAsync(Guid? documentId)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new ActionBlockedResultModel<DocumentViewModel>();
            var user = userRequest.Result;

            var response = new ResultModel<DocumentViewModel>();
            if (documentId == null)
                return new InvalidParametersResultModel<DocumentViewModel>();

            var document = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i => i.DocumentCategory)
                .FirstOrDefaultAsync(x => x.Id == documentId && x.TenantId == user.TenantId);

            if (document == null) return new NotFoundResultModel<DocumentViewModel>();

            response.IsSuccess = true;
            response.Result = document.Adapt<DocumentViewModel>();
            return response;

        }


        /// <summary>
        /// Get document by current user 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentViewModel>>> GetDocumentsByUserAsync()
        {
            var response = new ResultModel<IEnumerable<DocumentViewModel>>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new ActionBlockedResultModel<IEnumerable<DocumentViewModel>>();
            var user = userRequest.Result;

            var listDocuments = await _documentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Include(i => i.DocumentCategory)
                .Where(x => x.UserId == user.Id.ToGuid() && !x.IsDeleted).ToListAsync();

            if (listDocuments == null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<DocumentViewModel>>();

            response.IsSuccess = true;
            response.Result = listDocuments.Adapt<IEnumerable<DocumentViewModel>>();

            return response;
        }


        /// <summary>
        /// Get all document Version by document id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentVersionViewModel>>> GetAllDocumentVersionByIdAsync(Guid? documentId)
        {
            var result = new ResultModel<IEnumerable<DocumentVersionViewModel>>();

            if (documentId == null)
                return new InvalidParametersResultModel<IEnumerable<DocumentVersionViewModel>>();

            var listDocumentVersion = await _documentContext.DocumentVersions
                .Include(i => i.Document)
                .Where(x => x.DocumentId == documentId).OrderByDescending(o => o.VersionNumber).ToListAsync();

            if (listDocumentVersion == null || !listDocumentVersion.Any()) return new NotFoundResultModel<IEnumerable<DocumentVersionViewModel>>();

            result.IsSuccess = true;
            result.Result = listDocumentVersion.Adapt<List<DocumentVersionViewModel>>();
            return result;
        }


        /// <summary>
        /// Add new document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddDocumentAsync(AddDocumentViewModel model)
        {
            var result = new ResultModel();

            if (model == null || model.Title == null || model.DocumentCategoryId == null)
            {
                result.Errors.Add(new ErrorModel { Message = "entity is not valid" });
                result.IsSuccess = false;
                return result;
            }

            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                result.Errors.Add(new ErrorModel { Message = "User not found" });
                result.IsSuccess = false;
                return result;
            }
            var user = userRequest.Result;

            var newDocument = new Document
            {
                DocumentCategoryId = model.DocumentCategoryId,
                DocumentTypeId = model.DocumentTypeId,
                DocumentCode = model.DocumentCode,
                Title = model.Title,
                Description = model.Description,
                Group = model.Group,
                UserId = user.Id.ToGuid()
            };

            await _documentContext.Documents.AddAsync(newDocument);
            result = await _documentContext.PushAsync();


            if (result.IsSuccess)
            {
                result =  await AddNewDocumentVersionAsync(new AddNewVersionDocumentViewModel
                {
                    Comments = model.Comments,
                    DocumentId = newDocument.Id,
                    File = model.File,
                    IsMajorVersion = true,
                });
            }

            result.Result = newDocument.Id;
            return result;
        }

        /// <summary>
        /// Edit document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> EditDocumentAsync(AddDocumentViewModel model)
        {
            var result = new ResultModel();

            if (model == null)
            {
                result.Errors.Add(new ErrorModel { Message = "entity == null" });
                result.IsSuccess = false;
                return result;
            }

            var document = await _documentContext.Documents.FirstOrDefaultAsync(x => x.Id == model.DocumentId);

            if (document == null)
            {
                result.Errors.Add(new ErrorModel { Message = "document not fount" });
                result.IsSuccess = false;
                return result;
            }

            document.DocumentCode = model.DocumentCode;
            document.Title = model.Title;
            document.Description = model.Description;
            document.Group = model.Group;
            document.DocumentTypeId = model.DocumentTypeId;

            _documentContext.Documents.Update(document);
            result = await _documentContext.PushAsync();

            return result;
        }


        /// <summary>
        /// Add new document version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddNewDocumentVersionAsync(AddNewVersionDocumentViewModel model)
        {
            var result = new ResultModel();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                result.Errors.Add(new ErrorModel { Message = "User not found" });
                result.IsSuccess = false;
                return result;
            }
            var user = userRequest.Result;

            var document = await _documentContext.Documents.FirstOrDefaultAsync(x => x.Id == model.DocumentId);

            if (document == null)
            {
                result.Errors.Add(new ErrorModel { Message = "document not fount" });
                result.IsSuccess = false;
                return result;
            }


            Guid? fileId = null;

            if (model.File != null)
                fileId = FileManager.AddFile(new UploadFileViewModel { File = model.File }, user.Id.ToGuid()).Result;

            var newDocumentVersion = new DocumentVersion
            {
                DocumentId = model.DocumentId,
                FileStorageId = fileId,
                IsArhive = false,
                Comments = model.Comments,
                OwnerId = user.Id.ToGuid(),
                IsMajorVersion = model.IsMajorVersion,
                FileName = model.File?.FileName ?? ""
            };

            var lastVersion = await GetLastDocVersion(model.DocumentId);

            if (model.IsMajorVersion)
                newDocumentVersion.VersionNumber = (int)lastVersion + 1;
            else
                newDocumentVersion.VersionNumber = lastVersion + 0.1;

            await _documentContext.DocumentVersions.AddAsync(newDocumentVersion);
            result = await _documentContext.PushAsync();
            result.Result = newDocumentVersion.Id;
            return result;
        }

        /// <summary>
        /// Get last document version
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        protected virtual async Task<double> GetLastDocVersion(Guid documentId)
        {
            var listDocumentVersions = _documentContext.DocumentVersions.Where(x => x.DocumentId == documentId);

            if (!listDocumentVersions.Any()) return 0;

            var lastVersion = await listDocumentVersions.OrderBy(o => o.VersionNumber).LastOrDefaultAsync();

            return lastVersion.VersionNumber;
        }


        /// <summary>
        /// Get document by version Id
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<DocumentViewModel>> GetDocumentByVersionId(Guid? versionId)
        {
            var result = new ResultModel<DocumentViewModel>();
            if (versionId == null)
                return new InvalidParametersResultModel<DocumentViewModel>();

            var documentVersion = await _documentContext.DocumentVersions.Include(i => i.Document)
                .FirstOrDefaultAsync(x => x.Id == versionId);

            if (documentVersion == null) return new NotFoundResultModel<DocumentViewModel>();

            result.IsSuccess = true;
            result.Result = documentVersion.Document.Adapt<DocumentViewModel>();
            return result;

        }
       
    }
}
