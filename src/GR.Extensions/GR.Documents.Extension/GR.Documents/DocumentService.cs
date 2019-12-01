using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using GR.Files.Abstraction;
using GR.Files.Abstraction.Models.ViewModels;
using GR.Identity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.Documents
{
    public class DocumentService : IDocumentService
    {
        #region Injectable
        /// <summary>
        /// Inject db context 
        /// </summary>
        protected readonly IDocumentContext DocumentContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject file manager
        /// </summary>
        protected readonly IFileManager FileManager;

        #endregion

        public DocumentService(IDocumentContext documentContext, IUserManager<ApplicationUser> userManager, IFileManager fileManager)
        {
            DocumentContext = documentContext;
            _userManager = userManager;
            FileManager = fileManager;
        }


        /// <summary>
        /// Get all documents
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsAsync()
        {
            var user = await _userManager.GetCurrentUserAsync();
            var response = new ResultModel<IEnumerable<Document>>();

            var listDocuments = await DocumentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Where(x => x.TenantId == user.Result.TenantId)
                .ToListAsync();

            if (listDocuments is null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<Document>>();

            response.IsSuccess = true;
            response.Result = listDocuments;

            return response;
        }

        /// <summary>
        /// Get all document by type
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsByTypeIdAsync(Guid? typeId)
        {
            var result = new ResultModel<IEnumerable<Document>>();
            var user = await _userManager.GetCurrentUserAsync();

            if (typeId is null) return new InvalidParametersResultModel<IEnumerable<Document>>();

            var typeBd = await DocumentContext.DocumentTypes.FirstOrDefaultAsync(x => x.Id == typeId);
            if (typeBd is null) return new NotFoundResultModel<IEnumerable<Document>>();


            var documentsBd = await DocumentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Where(x => x.DocumentTypeId == typeId && x.TenantId == user.Result.TenantId && !x.IsDeleted).ToListAsync();

            if (documentsBd is null || !documentsBd.Any()) return new NotFoundResultModel<IEnumerable<Document>>();

            result.IsSuccess = true;
            result.Result = documentsBd;

            return result;
        }


        /// <summary>
        /// get documents by id an eliminate exist documents
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="listIgnireDocuments"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsByTypeIdAndListAsync(Guid? typeId,
            IEnumerable<Guid> listIgnireDocuments)
        {

            var result = new ResultModel<IEnumerable<Document>>();
            var user = await _userManager.GetCurrentUserAsync();

            if (typeId is null) return new InvalidParametersResultModel<IEnumerable<Document>>();

            var typeBd = await DocumentContext.DocumentTypes.FirstOrDefaultAsync(x => x.Id == typeId);
            if (typeBd is null) return new NotFoundResultModel<IEnumerable<Document>>();

            var documentsBd = await DocumentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Where(x => x.DocumentTypeId == typeId && !listIgnireDocuments.Contains(x.Id) && x.TenantId == user.Result.TenantId && !x.IsDeleted).ToListAsync();

            if (documentsBd is null || !documentsBd.Any()) return new NotFoundResultModel<IEnumerable<Document>>();

            result.IsSuccess = true;
            result.Result = documentsBd;

            return result;

        }

        /// <summary>
        /// Get  documents by list id
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsByListId(IEnumerable<Guid> listDocumetId)
        {
            var response = new ResultModel<IEnumerable<Document>>();
            var user = await _userManager.GetCurrentUserAsync();

            if (listDocumetId is null || !listDocumetId.Any()) return new InvalidParametersResultModel<IEnumerable<Document>>();

            var listDocuments = await DocumentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Where(x=> listDocumetId.Contains(x.Id) && x.TenantId == user.Result.TenantId && !x.IsDeleted)
                .ToListAsync();

            if (listDocuments is null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<Document>>();

            response.IsSuccess = true;
            response.Result = listDocuments;

            return response;
        }

       /// <summary>
       /// Delete documents by list id
       /// </summary>
       /// <returns></returns>
       public virtual async Task<ResultModel> DeleteDocumentsByListIdAsync(IEnumerable<Guid> listDocumetsId)
       {
           var response = new ResultModel();
           var user = await _userManager.GetCurrentUserAsync();

           if (listDocumetsId is null || !listDocumetsId.Any())
           {
               response.Errors.Add(new ErrorModel{Message = "List documents is null"});
               return response;
           }

           var listDocuments = await DocumentContext.Documents
               .Include(i => i.DocumentType)
               .Include(i => i.DocumentVersions)
               .Where(x => listDocumetsId.Contains(x.Id) && x.TenantId == user.Result.TenantId)
               .ToListAsync();          

           if (listDocuments is null || !listDocuments.Any())
           {
               response.Errors.Add(new ErrorModel { Message = "List documents  null" });
               return response;
           }

           foreach (var doc in listDocuments)
               doc.IsDeleted = true;


           DocumentContext.Documents.UpdateRange(listDocuments);
           response =  await DocumentContext.PushAsync();

           return response;
       }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Document>> GetDocumentsByIdAsync(Guid? documentId)
        {
            var user = await _userManager.GetCurrentUserAsync();

            var response = new ResultModel<Document>();
            if (documentId is null)
                return new InvalidParametersResultModel<Document>();

            var document = await DocumentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .FirstOrDefaultAsync(x => x.Id == documentId && x.TenantId == user.Result.TenantId);

            if (document is null) new NotFoundResultModel<Document>();

            response.IsSuccess = true;
            response.Result = document;
            return response;

        }


        /// <summary>
        /// Get document by current user 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Document>>> GetDocumentsByUserAsync()
        {
            var response = new ResultModel<IEnumerable<Document>>();
            var user = await _userManager.GetCurrentUserAsync();

            if (user is null)
                return new InvalidParametersResultModel<IEnumerable<Document>>();


            var listDocuments = await DocumentContext.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .Where(x => x.UserId == user.Result.Id.ToGuid() && !x.IsDeleted).ToListAsync();

            if (listDocuments is null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<Document>>();

            response.IsSuccess = true;
            response.Result = listDocuments;

            return response;
        }


        /// <summary>
        /// Get all document Version by document id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentVersion>>> GetAllDocumentVersionByIdAsync(Guid? documentId)
        {
            var result = new ResultModel<IEnumerable<DocumentVersion>>();

            if (documentId is null)
                return new InvalidParametersResultModel<IEnumerable<DocumentVersion>>();

            var listDocumentVersion = await DocumentContext.DocumentVersions
                .Include(i => i.Document)
                .Where(x => x.DocumentId == documentId).OrderByDescending(o => o.VersionNumber).ToListAsync();

            if (listDocumentVersion is null || !listDocumentVersion.Any()) return new NotFoundResultModel<IEnumerable<DocumentVersion>>();

            result.IsSuccess = true;
            result.Result = listDocumentVersion;
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

            if (model is null)
            {
                result.Errors.Add(new ErrorModel { Message = "entity is null" });
                result.IsSuccess = false;
                return result;
            }

            var user = await _userManager.GetCurrentUserAsync();

            if (user is null)
            {
                result.Errors.Add(new ErrorModel { Message = "user not fount" });
                result.IsSuccess = false;
                return result;
            }

            var newDocument = new Document
            {
                DocumentTypeId = model.DocumentTypeId,
                DocumentCode = model.DocumentCode,
                Title = model.Tile,
                Description = model.Description,
                Group = model.Group,
                UserId = user.Result.Id.ToGuid()
            };

            await DocumentContext.Documents.AddAsync(newDocument);

            Guid? fileId = null;

            if (model.File != null)
                fileId = FileManager.AddFile(new UploadFileViewModel { File = model.File }, user.Result.Id.ToGuid()).Result;

            var newDocumentVersion = new DocumentVersion
            {
                DocumentId = newDocument.Id,
                FileStorageId = fileId,
                VersionNumber = 1,
                IsArhive = false,
                Comments = model.Comments,
                OwnerId = user.Result.Id.ToGuid(),
                IsMajorVersion = true,
                Url = model.Url,
                FileName = model.File?.FileName ?? ""
            };

            await DocumentContext.DocumentVersions.AddAsync(newDocumentVersion);
            result = await DocumentContext.PushAsync();
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

            if (model is null)
            {
                result.Errors.Add(new ErrorModel { Message = "entity is null" });
                result.IsSuccess = false;
                return result;
            }

            var user = await _userManager.GetCurrentUserAsync();

            if (user is null)
            {
                result.Errors.Add(new ErrorModel { Message = "user not fount" });
                result.IsSuccess = false;
                return result;
            }

            var document = await DocumentContext.Documents.FirstOrDefaultAsync(x => x.Id == model.DocumentId);

            if (document is null)
            {
                result.Errors.Add(new ErrorModel { Message = "document not fount" });
                result.IsSuccess = false;
                return result;
            }

            document.DocumentCode = model.DocumentCode;
            document.Title = model.Tile;
            document.Description = model.Description;
            document.Group = model.Group;

            DocumentContext.Documents.Update(document);
            result = await DocumentContext.PushAsync();
            
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
            var user = await _userManager.GetCurrentUserAsync();

            if (user is null)
            {
                result.Errors.Add(new ErrorModel { Message = "user not fount" });
                result.IsSuccess = false;
                return result;
            }

            var document = await DocumentContext.Documents.FirstOrDefaultAsync(x => x.Id == model.DocumentId);

            if (document is null)
            {
                result.Errors.Add(new ErrorModel { Message = "document not fount" });
                result.IsSuccess = false;
                return result;
            }


            Guid? fileId = null;

            if (model.File != null)
                fileId = FileManager.AddFile(new UploadFileViewModel { File = model.File }, user.Result.Id.ToGuid()).Result;

            var newDocumentVersion = new DocumentVersion
            {
                DocumentId = model.DocumentId,
                FileStorageId = fileId,
                IsArhive = false,
                Comments = model.Comments,
                OwnerId = user.Result.Id.ToGuid(),
                IsMajorVersion = model.IsMajorVersion,
                FileName = model.File?.FileName ?? ""
            };

            var lastVersion = await GetLastDocVersion(model.DocumentId);

            if (model.IsMajorVersion)
                newDocumentVersion.VersionNumber = (int)lastVersion + 1;
            else
                newDocumentVersion.VersionNumber = lastVersion + 0.1;

            await DocumentContext.DocumentVersions.AddAsync(newDocumentVersion);
            result = await DocumentContext.PushAsync();
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
            var listDocumentVersions = DocumentContext.DocumentVersions.Where(x => x.DocumentId == documentId);

            if (!listDocumentVersions.Any())
                return 0;

            var lastVersion = await listDocumentVersions.OrderBy(o => o.VersionNumber).LastOrDefaultAsync();

            return lastVersion.VersionNumber;
        }
    }
}
