using GR.Documents.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using GR.Core.Helpers;
using GR.Documents.Abstractions.Models;
using System.Threading.Tasks;
using GR.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using GR.Core.Helpers.Responses;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.ViewModels.DocumentTypeViewModels;
using GR.Core;
using GR.Core.Abstractions;
using System.Linq;
using Mapster;

namespace GR.Documents
{
    public  class DocumentTypeService: IDocumentTypeService
    {

        #region Injectable

        /// <summary>
        /// Inject db context 
        /// </summary>
        private IDocumentContext _context;

        protected readonly IDataFilter _dataFilter;
        #endregion

        public DocumentTypeService(IDocumentContext context, IDataFilter dataFilter)
        {
            _context = context;
            _dataFilter = dataFilter;
        }

        /// <summary>
        /// Get all document type
        /// </summary>
        /// <returns></returns>
        public DTResult<DocumentTypeViewModel> GetAllDocumentType(DTParameters param)
        {

            var filtered = _dataFilter.FilterAbstractEntity<DocumentType, IDocumentContext>(_context, param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).Select(x =>
            {
                var listModel = x.Adapt<DocumentTypeViewModel>();
                return listModel;
            }).ToList();

            var result = new DTResult<DocumentTypeViewModel>
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

        public async Task<ResultModel> SaveDocumentTypeAsync(DocumentTypeViewModel model)
        {
            var code = (await _context.DocumentTypes.LastOrDefaultAsync())?.Code ?? 0;
            code++;

            model.Code = code;
            await _context.DocumentTypes.AddAsync(model);
            var dbResult = await _context.PushAsync();
            dbResult.Result = model;
            return dbResult;
        }
    }
}
