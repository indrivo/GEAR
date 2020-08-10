using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.IdentityDocuments.Abstractions.Models;
using GR.IdentityDocuments.Abstractions.ViewModels;

namespace GR.IdentityDocuments.Abstractions
{
    public interface IIdentityDocumentService
    {
        /// <summary>
        /// Get available document types
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<IdentityDocumentType>>> GetAvailableDocumentTypesAsync();

        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UploadDocumentAsync(UploadIdentityDocumentViewModel model);

        /// <summary>
        /// Upload documents
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        Task<ResultModel> UploadDocumentsAsync(IEnumerable<UploadIdentityDocumentViewModel> documents);
    }
}