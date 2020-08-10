using GR.Documents.Abstractions;
using System;
using GR.Core.Helpers;
using GR.Documents.Abstractions.Models;
using System.Threading.Tasks;
using GR.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;
using GR.Core;
using Mapster;
using System.Collections.Generic;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;

namespace GR.Documents
{
    [Author(Authors.DOROSENCO_ION, 1.1)]
    [Author(Authors.LUPEI_NICOLAE, 1.2, "Clean code")]
    public class DocumentTypeService : IDocumentTypeService
    {

        #region Injectable

        /// <summary>
        /// Inject db context 
        /// </summary>
        private readonly IDocumentContext _context;
        #endregion

        public DocumentTypeService(IDocumentContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all document type
        /// </summary>
        /// <returns></returns>
        public virtual async Task<DTResult<DocumentTypeViewModel>> GetAllDocumentTypesAsync(DTParameters param)
        {

            var filtered = await _context.DocumentTypes.GetPagedAsDtResultAsync(param);

            var result = new DTResult<DocumentTypeViewModel>
            {
                Draw = param.Draw,
                Data = filtered.Data.Adapt<List<DocumentTypeViewModel>>(),
                RecordsFiltered = filtered.RecordsFiltered,
                RecordsTotal = filtered.RecordsTotal
            };

            return result;
        }

        /// <summary>
        /// Get all document type async
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentType>>> GetAllDocumentTypeAsync()
        {
            var result = new ResultModel<IEnumerable<DocumentType>>();
            var listDocumentTypes = await _context.DocumentTypes.ToListAsync();

            result.IsSuccess = true;
            result.Result = listDocumentTypes;

            return result;
        }

        /// <summary>
        /// Get document by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<DocumentTypeViewModel>> GetDocumentTypeByIdAsync(Guid? id)
        {
            var result = new ResultModel<DocumentTypeViewModel>();

            if (id is null)
            {
                result.Errors.Add(new ErrorModel { Message = "Id is null" });
                result.IsSuccess = false;
                return result;
            }

            var documentType = await _context.DocumentTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (documentType is null)
            {
                result.Errors.Add(new ErrorModel { Message = "Document type not found" });
                result.IsSuccess = false;
                return result;
            }

            result.IsSuccess = true;
            result.Result = documentType.Adapt<DocumentTypeViewModel>();

            return result;
        }

        /// <summary>
        /// save document type async 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SaveDocumentTypeAsync(DocumentTypeViewModel model)
        {
            //model.Code = code;
            await _context.DocumentTypes.AddAsync(model);
            var dbResult = await _context.PushAsync();
            dbResult.Result = model;
            return dbResult;
        }


        /// <summary>
        /// Delete document type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteDocumentTypeAsync(Guid? id)
        {
            var result = new ResultModel();


            var document = await GetDocumentTypeByIdAsync(id);
            if (!document.IsSuccess)
            {
                result.Errors = document.Errors;
                result.IsSuccess = document.IsSuccess;
                return result;
            }

            _context.DocumentTypes.Remove(document.Result.Adapt<DocumentType>());
            result = await _context.PushAsync();
            return result;
        }

        /// <summary>
        /// Edit document type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<DocumentTypeViewModel>> EditDocumentTypeAsync(DocumentTypeViewModel model)
        {
            var result = new ResultModel<DocumentTypeViewModel>();
            var documentTypeRequest = await GetDocumentTypeByIdAsync(model.Id);

            if (!documentTypeRequest.IsSuccess)
            {
                return documentTypeRequest;
            }

            var documentBd = documentTypeRequest.Result.Adapt<DocumentType>();
            documentBd.Name = model.Name;

            _context.DocumentTypes.Update(documentBd);
            var resultPush = await _context.PushAsync();

            result.Errors = resultPush.Errors;
            result.IsSuccess = resultPush.IsSuccess;
            result.Result = model;
            return result;

        }
    }
}
