﻿using GR.Core.Helpers;
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
        /// Get all documents
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsAsync();

        /// <summary>
        /// Get documents by list id
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsByListId(IEnumerable<Guid> listDocumetId);

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
        Task<ResultModel<Document>> GetDocumentsByIdAsync(Guid? documentId);

        /// <summary>
        /// Get all document by type
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>

        Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsByTypeIdAsync(Guid? typeId);

        /// <summary>
        /// get documents by id an eliminate exist documents
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="listIgnireDocuments"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Document>>> GetAllDocumentsByTypeIdAndListAsync(Guid? typeId,
            IEnumerable<Guid> listIgnireDocuments);


        /// <summary>
        /// Get all document create by current user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Document>>> GetDocumentsByUserAsync();

        /// <summary>
        /// Get al document version by document id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DocumentVersion>>> GetAllDocumentVersionByIdAsync(Guid? documentId);

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
    }
}
