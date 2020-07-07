using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Infrastructure.Services
{
    public class ProductAttributeService : IProductAttributeService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ICommerceContext _context;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        public ProductAttributeService(ICommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get product attributes
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<ProductAttributeItemViewModel>>> GetProductAttributesAsync(Guid productId)
        {
            var attrs = await _context.ProductAttributes
                .Include(x => x.ProductAttribute)
                .Where(x => x.ProductId == productId).ToListAsync();
            var mapped = _mapper.Map<IEnumerable<ProductAttributeItemViewModel>>(attrs);
            return new SuccessResultModel<IEnumerable<ProductAttributeItemViewModel>>(mapped);
        }

        /// <summary>
        /// Get attributes for filters
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<FilterAttributeValuesViewModel>>> GetAttributesForFiltersAsync()
        {
            var attributes = await _context.ProductAttribute
                .Include(x => x.AttributeGroup)
                .ToListAsync();

            var data = attributes.Adapt<IEnumerable<FilterAttributeValuesViewModel>>().ToList();
            foreach (var item in data)
            {
                item.Values = _context.ProductAttributes
                    .Where(m => m.ShowInFilters && m.ProductAttributeId == item.Id)
                    .Select(x => x.Value)
                    .Distinct();
            }

            return new SuccessResultModel<IEnumerable<FilterAttributeValuesViewModel>>(data);
        }

        /// <summary>
        /// Remove product attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public async Task<ResultModel> RemoveAttributeAsync(Guid? productId, Guid? attributeId)
        {
            if (productId == null || attributeId == null) return new InvalidParametersResultModel();
            var result = await _context.ProductAttributes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductAttributeId == attributeId && x.ProductId == productId);

            if (result == null) return new NotFoundResultModel();
            _context.ProductAttributes.Remove(result);
            var dbResult = await _context.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Add or update product attributes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddOrUpdateProductAttributesAsync(IEnumerable<ProductAttributesViewModel> model)
        {
            foreach (var item in model?.ToList() ?? new List<ProductAttributesViewModel>())
            {
                var attribute = _context.ProductAttributes
                    .FirstOrDefault(x =>
                        x.ProductAttributeId == item.ProductAttributeId && x.ProductId == item.ProductId);

                if (attribute != null)
                {
                    _context.ProductAttributes.Remove(attribute);
                }

                await _context.ProductAttributes.AddAsync(item);
            }

            var dbResult = await _context.PushAsync();
            return dbResult;
        }
    }
}
