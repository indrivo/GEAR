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
            private IDocumentContext _context;

            /// <summary>
            /// Inject user manager
            /// </summary>
            private readonly IUserManager<ApplicationUser> _userManager;

            private IFileManager _fileManager;

        #endregion

        public DocumentService(IDocumentContext context, IUserManager<ApplicationUser> userManager,IFileManager fileManager)
        {
            _context = context;
            _userManager = userManager;
            _fileManager = fileManager;
        }


        /// <summary>
        /// Get all documents
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsAsync()
        {
            var response = new ResultModel<IEnumerable<Document>>();

            var listDocuments = await _context.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .ThenInclude(i => i.FileStorage)
                .ToListAsync();

            if (listDocuments is null || !listDocuments.Any()) return new NotFoundResultModel<IEnumerable<Document>>();

            response.IsSuccess = true;
            response.Result = listDocuments;

            return response;
        }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Document>> GetDocumentsByIdAsync(Guid? documentId)
        {

            var response = new ResultModel<Document>();
            if (documentId is null)
                return new InvalidParametersResultModel<Document>();

            var document = await _context.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .ThenInclude(i => i.FileStorage)
                .FirstOrDefaultAsync(x => x.Id == documentId);

            if (document is null) new NotFoundResultModel<Document>();

            response.IsSuccess = true;
            response.Result = document;
            return response;

        }


        /// <summary>
        /// Get document by current user 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Document>>> GetDocumentsByUserAsync()
        {
            var response = new ResultModel<IEnumerable<Document>>();
            var user = await _userManager.GetCurrentUserAsync();

            if(user is null)
                return new InvalidParametersResultModel<IEnumerable<Document>>();


            var listDocuments = await _context.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .ThenInclude(i => i.FileStorage)
                .Where(x => x.UserId == user.Result.Id.ToGuid()).ToListAsync();

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
        public async Task<ResultModel<IEnumerable<DocumentVersion>>> GetAllDocumentVersionByIdAsync(Guid? documentId)
        {
            var result = new ResultModel<IEnumerable<DocumentVersion>>();

            if (documentId is null)
                return new InvalidParametersResultModel<IEnumerable<DocumentVersion>>();

            var listDocumentVersion = await _context.DocumentVersions
                .Include(i => i.Document)
                .Include(i => i.FileStorage)
                .Where(x => x.DocumentId == documentId).ToListAsync();

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
        public async Task<ResultModel> AddDocumentAsync(AddDocumentViewModel model)
        {
            var result = new ResultModel();

            if (model is null)
            {
                result.Errors.Add(new ErrorModel {Message = "entity is null"});
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
                Description =  model.Description,
                Group = model.Group,
                UserId =  user.Result.Id.ToGuid()
            };

            await _context.Documents.AddAsync(newDocument);
            
            var fileId =  _fileManager.AddFile(new UploadFileViewModel {File = model.File}, user.Result.Id.ToGuid()).Result;

            var newDocumentVersion =  new DocumentVersion { };


            return result;
        }

    }
}
