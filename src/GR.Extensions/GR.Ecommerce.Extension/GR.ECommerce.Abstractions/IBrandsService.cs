using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.BrandViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface IBrandsService
    {
        /// <summary>
        /// Get brands
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Brand>>> GetBrandsAsync();

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<Brand>> GetBrandsWithPaginationAsync(DTParameters parameters);

        /// <summary>
        /// Create new brand
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateNewBrandAsync([Required]CreateBrandViewModel model);

        /// <summary>
        /// Update brand
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateBrandAsync([Required]UpdateBrandViewModel model);

        /// <summary>
        /// Add new brand
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddBrandAsync(Brand brand);
    }
}