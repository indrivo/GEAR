using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Api.Controllers
{
    /// <summary>
    /// This is a products api
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/products/[action]")]
    public class ProductsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        #endregion

        public ProductsApiController(IProductService<Product> productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get products with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(DTResult<ProductsPaginatedViewModel>))]
        public async Task<JsonResult> GetProductsWithPagination(DTParameters parameters)
            => await JsonAsync(_productService.GetProductsWithPaginationAsync(parameters));

        /// <summary>
        /// Remove variation option
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> GetPriceByVariation([Required] ProductPriceVariationViewModel model)
            => await JsonAsync(_productService.GetPriceByVariationAsync(model));

        /// <summary>
        /// Save product variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdateProductVariation([FromBody] UpdateProductVariationViewModel model)
            => await JsonAsync(_productService.AddOrUpdateVariationAsync(model));

        /// <summary>
        /// Get product variations 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<IEnumerable<ProductVariationViewModel>>))]
        public async Task<JsonResult> GetProductVariations(Guid productId)
            => await JsonAsync(_productService.GetProductVariationsAsync(productId));

        /// <summary>
        /// Get product variation by id
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<ProductVariationViewModel>))]
        public async Task<JsonResult> GetProductVariationById(Guid productId, Guid variationId)
            => await JsonAsync(_productService.GetProductVariationByIdAsync(productId, variationId));

        /// <summary>
        /// Remove variation option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveProductVariationOption([Required] Guid? productId, [Required] Guid? variationId)
            => await JsonAsync(_productService.RemoveVariationOptionAsync(productId, variationId));

        /// <summary>
        /// Get products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<Product>>))]
        public async Task<JsonResult> GetProductsWithFilters([Required] ProductsFilterRequest model)
            => await JsonAsync(_productService.GetProductsWithFiltersAsync(model));

        /// <summary>
        /// Get commerce statistic
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<SalesStatisticViewModel>))]
        public async Task<JsonResult> GetCommerceGeneralStatistic(DateTime? startDate, DateTime? endDate) =>
            await JsonAsync(_productService.GetCommerceGeneralStatisticAsync(startDate, endDate));

        /// <summary>
        /// Get year report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Dictionary<int, object>>))]
        public async Task<JsonResult> GetYearReport() =>
            await JsonAsync(_productService.GetYearReportAsync());

        /// <summary>
        /// Get global currency
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Currency>))]
        public async Task<JsonResult> GetGlobalCurrency() =>
            Json(await _productService.GetGlobalCurrencyAsync());


        /// <summary>
        /// Set global currency
        /// </summary>
        /// <param name="currencyIdentifier"></param>
        /// <returns></returns>
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Currency>))]
        public async Task<JsonResult> SetGlobalCurrency(string currencyIdentifier) =>
            Json(await _productService.SetGlobalCurrencyAsync(currencyIdentifier));
    }
}
