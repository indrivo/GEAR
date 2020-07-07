using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductCategoriesViewModels;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Infrastructure.Services
{
    public class ProductCategoryService : IProductCategoryService
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

        public ProductCategoryService(ICommerceContext commerceContext, IMapper mapper)
        {
            _commerceContext = commerceContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> CreateCategoryAsync(CreateCategoryViewModel model)
        {
            var validate = ModelValidator.IsValid<CreateCategoryViewModel, Guid>(model);
            if (!validate.IsSuccess) return validate;
            var mapped = _mapper.Map<Category>(model);
            return await _commerceContext.AddAsync<Category, Guid>(mapped);
        }

        /// <summary>
        /// Get categories with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<GetProductCategoryViewModel>> GetCategoriesWithPaginationAsync(DTParameters parameters)
        {
            var paginated = await _commerceContext.Categories.GetPagedAsDtResultAsync(parameters);
            return _mapper.Map<DTResult<GetProductCategoryViewModel>>(paginated);
        }

        /// <summary>
        /// Get active categories 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GetProductCategoryViewModel>>> GetActiveCategoriesAsync()
        {
            var categories = await _commerceContext.Categories.NonDeleted().ToListAsync();
            var mapped = _mapper.Map<IEnumerable<GetProductCategoryViewModel>>(categories);
            return new SuccessResultModel<IEnumerable<GetProductCategoryViewModel>>(mapped);
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GetProductCategoryViewModel>>> GetAllCategoriesAsync()
        {
            var categories = await _commerceContext.Categories.ToListAsync();
            var mapped = _mapper.Map<IEnumerable<GetProductCategoryViewModel>>(categories);
            return new SuccessResultModel<IEnumerable<GetProductCategoryViewModel>>(mapped);
        }

        /// <summary>
        /// Add or update product categories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddOrUpdateProductCategoriesAsync(IEnumerable<ProductCategoriesViewModel> model)
        {
            foreach (var item in model)
            {
                if (_commerceContext.ProductCategories.Any(x =>
                    x.CategoryId == item.CategoryId && x.ProductId == item.ProductId))
                {
                    if (!item.Checked)
                    {
                        _commerceContext.ProductCategories.Remove(item);
                    }
                }
                else
                {
                    if (item.Checked)
                    {
                        await _commerceContext.ProductCategories.AddAsync(item);
                    }
                }
            }

            var dbResult = await _commerceContext.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Get product categories
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<ProductCategory>>> GetProductCategoriesAsync(Guid productId)
        {
            var productCategories = await
                _commerceContext.ProductCategories.Where(x => x.ProductId == productId).ToListAsync();

            return new SuccessResultModel<IEnumerable<ProductCategory>>(productCategories);
        }
    }
}