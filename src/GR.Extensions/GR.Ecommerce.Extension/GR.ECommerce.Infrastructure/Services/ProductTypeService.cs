using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductTypeViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Infrastructure.Services
{
    public class ProductTypeService : IProductTypeService
    {
        #region Injectable

        /// <summary>
        /// Inject commerce context
        /// </summary>
        private readonly ICommerceContext _commerceContext;

        #endregion

        public ProductTypeService(ICommerceContext commerceContext)
        {
            _commerceContext = commerceContext;
        }

        /// <summary>
        /// Get product types
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<ProductType>>> GetProductTypesAsync()
        {
            var productTypes = await _commerceContext.ProductTypes.ToListAsync();
            return new SuccessResultModel<IEnumerable<ProductType>>(productTypes);
        }

        /// <summary>
        /// get product type by id
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ProductType>> GetProductTypeByIdAsync(Guid productTypeId)
            => await _commerceContext.FindByIdAsync<ProductType, Guid>(productTypeId);

        /// <summary>
        /// Get product types with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<ProductType>> GetProductTypesWithPaginationAsync(DTParameters parameters)
            => await _commerceContext.ProductTypes.GetPagedAsDtResultAsync(parameters);

        /// <summary>
        /// Add product type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddProductTypeAsync(CreateProductTypeViewModel model)
        {
            var validate = ModelValidator.IsValid<CreateProductTypeViewModel, Guid>(model);
            if (!validate.IsSuccess) return validate;

            return await _commerceContext.AddAsync<ProductType, Guid>(new ProductType
            {
                Name = model.Name,
                DisplayName = model.DisplayName
            });
        }
    }
}