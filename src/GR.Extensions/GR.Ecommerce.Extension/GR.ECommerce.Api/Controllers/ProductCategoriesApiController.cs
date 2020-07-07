using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductCategoriesViewModels;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Api.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/product-categories/[action]")]
    public class ProductCategoriesApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject category service
        /// </summary>
        private readonly IProductCategoryService _categoryService;

        #endregion


        public ProductCategoriesApiController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Create category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [JsonProduces(typeof(ResultModel<Guid>))]
        public async Task<JsonResult> CreateCategory(CreateCategoryViewModel model)
            => await JsonAsync(_categoryService.CreateCategoryAsync(model));

        /// <summary>
        /// Get categories with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(DTResult<CreateCategoryViewModel>))]
        public async Task<JsonResult> GetCategoriesWithPagination(DTParameters parameters)
            => await JsonAsync(_categoryService.GetCategoriesWithPaginationAsync(parameters));

        /// <summary>
        /// Edit map product categories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdateProductCategories([FromBody] IEnumerable<ProductCategoriesViewModel> model)
            => await JsonAsync(_categoryService.AddOrUpdateProductCategoriesAsync(model));

        /// <summary>
        /// Get product categories
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<IEnumerable<ProductCategory>>))]
        public async Task<JsonResult> GetProductCategories(Guid productId)
            => await JsonAsync(_categoryService.GetProductCategoriesAsync(productId));

        /// <summary>
        /// Get product categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<GetProductCategoryViewModel>>))]
        public async Task<JsonResult> GetActiveCategories()
            => await JsonAsync(_categoryService.GetActiveCategoriesAsync());

        /// <summary>
        /// Get product categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<GetProductCategoryViewModel>>))]
        public async Task<JsonResult> GetAllCategories()
            => await JsonAsync(_categoryService.GetAllCategoriesAsync());
    }
}