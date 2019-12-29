using GR.Core;
using GR.Core.Helpers;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR.Documents.Abstractions
{
    public interface IDocumentService
    {
        /// <summary>
        /// Get all documents  from table model
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        DTResult<DocumentViewModel> GetAllDocument(DTParameters param);

        /// <summary>
        /// Get all documents
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsAsync();

        /// <summary>
        /// Get documents by list id
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsByListId(IEnumerable<Guid> listDocumetId);

        /// <summary>
        /// Delete documents by list id
        /// </summary>
        /// <param name="listDocumetsId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteDocumentsByListIdAsync(IEnumerable<Guid> listDocumetsId);


        /// <summary>
        /// Get document by id async
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        Task<ResultModel<DocumentViewModel>> GetDocumentsByIdAsync(Guid? documentId);

        /// <summary>
        /// Get all document by type
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>

        Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsByTypeIdAsync(Guid? typeId);

        /// <summary>
        /// get documents by id an eliminate exist documents
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="listIgnireDocuments"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentViewModel>>> GetAllDocumentsByCategoryIdAndListAsync(Guid? typeId,
            IEnumerable<Guid> listIgnireDocuments);


        /// <summary>
        /// Get all document create by current user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentViewModel>>> GetDocumentsByUserAsync();

        /// <summary>
        /// Get al document version by document id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentVersionViewModel>>> GetAllDocumentVersionByIdAsync(Guid? documentId);

        /// <summary>
        /// Add new document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddDocumentAsync(AddDocumentViewModel model);

        /// <summary>
        /// Add new document version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddNewDocumentVersionAsync(AddNewVersionDocumentViewModel model);


        /// <summary>
        /// Edit document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> EditDocumentAsync(AddDocumentViewModel model);

        /// <summary>
        /// Get document by version id
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        Task<ResultModel<DocumentViewModel>> GetDocumentByVersionId(Guid? versionId);
    }
}
