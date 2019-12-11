using GR.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentCategoryViewModels;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;

namespace GR.Documents.Abstractions
{
    public interface IDocumentCategoryService
    {
        /// <summary>
        /// Get all document category
        /// </summary>
        /// <returns></returns>
        DTResult<DocumentCategoryViewModel> GetAllDocumentCategory(DTParameters param);

        /// <summary>
        /// Get all document category async
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentCategoryViewModel>>> GetAllDocumentCategoryAsync();

        /// <summary>
        /// Get document category by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<DocumentCategoryViewModel>> GetDocumentCategoryByIdAsync(Guid? id);

        /// <summary>
        /// Save document category
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> SaveDocumentCategoryAsync(DocumentCategoryViewModel model);

        /// <summary>
        /// Delete document category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteDocumentCategoryAsync(Guid? id);

        /// <summary>
        /// Edit document category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<DocumentCategoryViewModel>> EditDocumentCategoryAsync(DocumentCategoryViewModel model);
    }
}
