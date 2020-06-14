using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
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
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Razor.Api
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
        /// Remove variation option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveOption([Required] Guid? productId, [Required] Guid? variationId)
            => await JsonAsync(_productService.RemoveVariationOptionAsync(productId, variationId));

        /// <summary>
        /// Remove variation option
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> GetPriceByVariation([Required]ProductPriceVariationViewModel model)
            => await JsonAsync(_productService.GetPriceByVariationAsync(model));

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

        /// <summary>
        /// Edit product attributes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> EditProductAttributes([FromBody] IEnumerable<ProductAttributesViewModel> model)
        {
            foreach (var item in model?.ToList() ?? new List<ProductAttributesViewModel>())
            {
                var attribute = _productService.Context.ProductAttributes
                    .FirstOrDefault(x =>
                        x.ProductAttributeId == item.ProductAttributeId && x.ProductId == item.ProductId);

                if (attribute != null)
                {
                    _productService.Context.ProductAttributes.Remove(attribute);
                }

                _productService.Context.ProductAttributes.Add(item);
            }

            var dbResult = await _productService.Context.PushAsync();
            return Json(dbResult);
        }

        /// <summary>
        /// Save product variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> SaveProductVariation([FromBody] ProductVariationViewModel model)
        {
            var response = new ResultModel();
            if (!ModelState.IsValid)
            {
                response.Errors = ModelState.ToResultModelErrors().ToList();
                return Json(response);
            }

            var prod = await _productService.Context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == model.ProductId);

            if (prod != null)
            {
                var variation = await _productService.Context.ProductVariations
                    .FirstOrDefaultAsync(i => i.Id == model.VariationId && i.ProductId == model.ProductId);


                if (variation != null)
                {
                    variation.Price = model.Price;
                    _productService.Context.ProductVariations.Update(variation);

                    var listProductVariationDetails =
                        _productService.Context.ProductVariationDetails.Where(x => x.ProductVariationId == variation.Id);

                    _productService.Context.ProductVariationDetails.RemoveRange(listProductVariationDetails);
                }
                else
                {
                    variation = new ProductVariation
                    {
                        ProductId = model.ProductId,
                        Price = model.Price,
                    };

                    _productService.Context.ProductVariations.Add(variation);
                }

                var variationDetails = model.ProductVariationDetails.Select(x => new ProductVariationDetail()
                {
                    ProductVariationId = variation.Id,
                    Value = x.Value,
                    ProductOptionId = x.ProductOptionId
                });

                await _productService.Context.ProductVariationDetails.AddRangeAsync(variationDetails);
            }

            var dbResult = await _productService.Context.PushAsync();
            return Json(dbResult);
        }


        [HttpGet]
        public JsonResult GetProductVariation(string productId) 
            => Json(_productService.Context.ProductVariations
            .Include(x => x.ProductVariationDetails).ThenInclude(x => x.ProductOption)
            .Where(x => x.ProductId == productId.ToGuid())
            .Select(x => new
            {
                VariationId = x.Id,
                x.Price,
                VariationDetails = x.ProductVariationDetails.Select(s => new { s.Value, Option = s.ProductOption.Name })
            }));


        [HttpGet]
        public JsonResult GetProductVariationById(string productId, string variationId) => Json(_productService.Context
            .ProductVariations
            .Include(x => x.ProductVariationDetails).ThenInclude(x => x.ProductOption)
            .Where(x => x.ProductId == productId.ToGuid() && x.Id == variationId.ToGuid())
            .Select(x => new
            {
                x.ProductId,
                VariationId = x.Id,
                x.Price,
                VariationDetails = x.ProductVariationDetails.Select(s => new { s.Value, Option = s.ProductOption.Name, optionId = s.ProductOptionId })
            }).FirstOrDefault());


        [HttpGet]
        public JsonResult GetProductAttributes(string productId) => Json(_productService.Context.ProductAttributes
            .Include(x => x.ProductAttribute)
            .Where(x => x.ProductId == productId.ToGuid())
            .Select(x => new
            {
                AttributeId = x.ProductAttributeId,
                Label = x.ProductAttribute.Name,
                x.Value,
                x.IsAvailable,
                x.IsPublished,
                x.ShowInFilters
            }));


        /// <summary>
        /// Remove attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveAttribute(Guid productId, Guid attributeId)
            => await JsonAsync(_productService.RemoveAttributeAsync(productId, attributeId));

        /// <summary>
        /// Edit map product categories
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> EditProductCategories([FromBody] IEnumerable<ProductCategoriesViewModel> model)
        {
            foreach (var item in model)
            {
                if (_productService.Context.ProductCategories.Any(x =>
                    x.CategoryId == item.CategoryId && x.ProductId == item.ProductId))
                {
                    if (!item.Checked)
                    {
                        _productService.Context.ProductCategories.Remove(item);
                    }
                }
                else
                {
                    if (item.Checked)
                    {
                        _productService.Context.ProductCategories.Add(item);
                    }
                }
            }

            var dbResult = await _productService.Context.PushAsync();
            return Json(dbResult);
        }

        [HttpGet]
        public JsonResult GetProductCategories(Guid productId)
            => Json(_productService.Context.ProductCategories.Where(x => x.ProductId == productId));

        /// <summary>
        /// Get product categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<Category>>))]
        public async Task<JsonResult> GetCategories()
        {
            var categories = await _productService.Context.Categories.NonDeleted().ToListAsync();
            return Json(new SuccessResultModel<IEnumerable<Category>>(categories));
        }

        /// <summary>
        /// Get products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<Product>>))]
        public async Task<JsonResult> GetProductsWithFilters([Required] ProductsFilterRequest model)
            => await JsonAsync(_productService.GetProductsWithFiltersAsync(model));

        /// <summary>
        /// Get Brands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<Brand>>))]
        public async Task<JsonResult> GetBrands()
            => await JsonAsync(_productService.GetAllBrandsAsync());

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<FilterAttributeValuesViewModel>>))]
        public async Task<JsonResult> GetAttributes()
            => await JsonAsync(_productService.GetAttributesForFiltersAsync());
    }
}
