using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductCategoriesViewModels;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface IProductCategoryService
    {
        /// <summary>
        /// Create category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateCategoryAsync(CreateCategoryViewModel model);

        /// <summary>
        /// Get categories with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<GetProductCategoryViewModel>> GetCategoriesWithPaginationAsync(DTParameters parameters);

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GetProductCategoryViewModel>>> GetAllCategoriesAsync();

        /// <summary>
        /// Get active categories
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GetProductCategoryViewModel>>> GetActiveCategoriesAsync();

        /// <summary>
        /// Add or update product categories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddOrUpdateProductCategoriesAsync(IEnumerable<ProductCategoriesViewModel> model);

        /// <summary>
        /// Get product categories
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<ProductCategory>>> GetProductCategoriesAsync(Guid productId);
    }
}