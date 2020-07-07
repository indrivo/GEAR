using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.BrandViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Infrastructure.Services
{
    public class BrandService : IBrandsService
    {
        #region Injectable

        /// <summary>
        /// Inject commerce context
        /// </summary>
        private readonly ICommerceContext _commerceContext;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        public BrandService(ICommerceContext commerceContext, IMapper mapper)
        {
            _commerceContext = commerceContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Get brands
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Brand>>> GetBrandsAsync()
        {
            var brands = await _commerceContext.Brands.ToListAsync();
            return new SuccessResultModel<IEnumerable<Brand>>(brands);
        }
        
        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<Brand>> GetBrandsWithPaginationAsync(DTParameters parameters)
            => await _commerceContext.Brands.GetPagedAsDtResultAsync(parameters);

        /// <summary>
        /// Create new brand
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> CreateNewBrandAsync(CreateBrandViewModel model)
        {
            var validate = ModelValidator.IsValid<CreateBrandViewModel, Guid>(model);
            if (!validate.IsSuccess) return validate;
            var mapped = _mapper.Map<Brand>(model);
            await _commerceContext.Brands.AddAsync(mapped);
            var dbResult = await _commerceContext.PushAsync();
            return dbResult.Map(mapped.Id);
        }

        /// <summary>
        /// Add new brand
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddBrandAsync(Brand brand)
        {
            if (brand == null) return new InvalidParametersResultModel<Guid>();
            var modelState = ModelValidator.IsValid(brand);
            if (!modelState.IsSuccess) return modelState.Map<Guid>();
            await _commerceContext.Brands.AddAsync(brand);
            var dbResponse = await _commerceContext.PushAsync();
            return dbResponse.Map(brand.Id);
        }

        /// <summary>
        /// Update brand
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateBrandAsync(UpdateBrandViewModel model)
        {
            var validate = ModelValidator.IsValid(model);
            if (!validate.IsSuccess) return validate;
            return await _commerceContext.UpdateAsync<Brand, Guid>(model.Id, options =>
            {
                options.Name = model.Name;
                options.DisplayName = model.DisplayName;
            });
        }
    }
}