using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface IProductAttributeService
    {
        /// <summary>
        /// Get product attributes
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<ProductAttributeItemViewModel>>> GetProductAttributesAsync(Guid productId);

        /// <summary>
        /// Get attributes for filters
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<FilterAttributeValuesViewModel>>> GetAttributesForFiltersAsync();

        /// <summary>
        /// Add or update product attributes 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddOrUpdateProductAttributesAsync(IEnumerable<ProductAttributesViewModel> model);

        /// <summary>
        /// Remove attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveAttributeAsync(Guid? productId, Guid? attributeId);
    }
}