using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductTypeViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface IProductTypeService
    {
        /// <summary>
        /// Get product types
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<ProductType>>> GetProductTypesAsync();

        /// <summary>
        /// Get product type by id
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        Task<ResultModel<ProductType>> GetProductTypeByIdAsync(Guid productTypeId);

        /// <summary>
        /// Get product types with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<ProductType>> GetProductTypesWithPaginationAsync(DTParameters parameters);

        /// <summary>
        /// Create new product type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddProductTypeAsync(CreateProductTypeViewModel model);
    }
}