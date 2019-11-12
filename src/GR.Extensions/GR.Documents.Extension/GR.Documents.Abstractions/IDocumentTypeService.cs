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
        /// Save document type
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> SaveDocumentTypeAsync(DocumentTypeViewModel model);
    }
}
