using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentCategoryViewModels;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.Documents
{
    [Author(Authors.DOROSENCO_ION, 1.1)]
    public class DocumentCategoryService: IDocumentCategoryService
    {

        #region Injectable

        /// <summary>
        /// Inject db context 
        /// </summary>
        private readonly IDocumentContext _context;

        protected readonly IDataFilter DataFilter;
        #endregion

        public DocumentCategoryService(IDocumentContext context, IDataFilter dataFilter)
        {
            _context = context;
            DataFilter = dataFilter;
        }

        /// <summary>
        /// Get all document category
        /// </summary>
        /// <returns></returns>
        public virtual DTResult<DocumentCategoryViewModel> GetAllDocumentCategory(DTParameters param)
        {
            var filtered = DataFilter.FilterAbstractEntity<DocumentCategory, IDocumentContext>(_context, param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).Select(x =>
            {
                var listModel = x.Adapt<DocumentCategoryViewModel>();
                return listModel;
            }).ToList();

            var result = new DTResult<DocumentCategoryViewModel>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            //var result = new ResultModel<IEnumerable<DocumentType>>();
            //var listDocumentTypes = await _context.DocumentTypes.ToListAsync();

            //if(listDocumentTypes is null || !listDocumentTypes.Any())
            //    new NotFoundResultModel<IEnumerable<DocumentType>>();

            //result.IsSuccess = true;
            //result.Result = listDocumentTypes;

            return result;
        }

        /// <summary>
        /// Get all document category async
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DocumentCategoryViewModel>>> GetAllDocumentCategoryAsync()
        {
            var result = new ResultModel<IEnumerable<DocumentCategoryViewModel>>();
            var listDocumentTypes = await _context.DocumentCategories.ToListAsync();

            result.IsSuccess = true;
            result.Result = listDocumentTypes.Adapt<IEnumerable<DocumentCategoryViewModel>>();

            return result;
        }


        /// <summary>
        /// Get document category by id async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<DocumentCategoryViewModel>> GetDocumentCategoryByIdAsync(Guid? id)
        {
            var result = new ResultModel<DocumentCategoryViewModel>();

            if (id is null)
            {
                result.Errors.Add(new ErrorModel { Message = "Id is null" });
                result.IsSuccess = false;
                return result;
            }

            var documentType = await _context.DocumentCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (documentType is null)
            {
                result.Errors.Add(new ErrorModel { Message = "Document type not found" });
                result.IsSuccess = false;
                return result;
            }

            result.IsSuccess = true;
            result.Result = documentType.Adapt<DocumentCategoryViewModel>();

            return result;
        }

        /// <summary>
        /// Save document category
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> SaveDocumentCategoryAsync(DocumentCategoryViewModel model)
        {

            var code = (await _context.DocumentCategories.OrderByDescending(o=> o.Created).FirstOrDefaultAsync())?.Code ?? 0;
            code++;

            model.Code = code;
            await _context.DocumentCategories.AddAsync(model);
            var dbResult = await _context.PushAsync();
            dbResult.Result = model;
            return dbResult;

        }

        /// <summary>
        /// Delete document category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteDocumentCategoryAsync(Guid? id)
        {
            var result = new ResultModel();

            var document = await GetDocumentCategoryByIdAsync(id);
            if (!document.IsSuccess)
            {
                result.Errors = document.Errors;
                result.IsSuccess = document.IsSuccess;
                return result;
            }

            _context.DocumentCategories.Remove(document.Result.Adapt<DocumentCategory>());
            result = await _context.PushAsync();
            return result;
        }

        /// <summary>
        /// Edit document category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<DocumentCategoryViewModel>> EditDocumentCategoryAsync(DocumentCategoryViewModel model)
        {
            var result = new ResultModel<DocumentCategoryViewModel>();
            var documentTypeRequest = await GetDocumentCategoryByIdAsync(model.Id);

            if (!documentTypeRequest.IsSuccess)
            {
                return documentTypeRequest;
            }

            var documentBd = documentTypeRequest.Result.Adapt<DocumentCategory>();
            documentBd.Name = model.Name;

            _context.DocumentCategories.Update(documentBd);
            var resultPush = await _context.PushAsync();

            result.Errors = resultPush.Errors;
            result.IsSuccess = resultPush.IsSuccess;
            result.Result = model;
            return result;
        }

    }
}
