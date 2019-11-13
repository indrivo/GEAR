using GR.Core;
using GR.Core.Helpers;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GR.Documents.Abstractions
{
    public interface IDocumentTypeService
    {

        /// <summary>
        /// Get all document types
        /// </summary>
        /// <returns></returns>
        DTResult<DocumentTypeViewModel> GetAllDocumentType(DTParameters param);

        /// <summary>
        /// Get all document types async
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentType>>> GetAllDocumentTypeAsync();

        /// <summary>
        /// Get document type by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<DocumentTypeViewModel>> GetDocumentTypeByIdAsync(Guid? id);

        /// <summary>
        /// Save document type
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> SaveDocumentTypeAsync(DocumentTypeViewModel model);

        /// <summary>
        /// Delete document type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteDocumentTypeAsync(Guid? id);

        /// <summary>
        /// Edit document type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<DocumentTypeViewModel>> EditDocumentTypeAsync(DocumentTypeViewModel model);
    }
}
