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
using GR.ECommerce.Abstractions.ViewModels.ProductTypeViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Api.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/product-type/[action]")]
    public class ProductsTypeApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject product type service
        /// </summary>
        private readonly IProductTypeService _productTypeService;

        #endregion

        public ProductsTypeApiController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        /// <summary>
        /// Get product types with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(DTResult<ProductType>))]
        public async Task<JsonResult> GetProductTypesWithPagination(DTParameters parameters)
            => await JsonAsync(_productTypeService.GetProductTypesWithPaginationAsync(parameters));

        /// <summary>
        /// Get product type by id
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<ProductType>))]
        public async Task<JsonResult> GetProductTypeById(Guid productTypeId)
            => await JsonAsync(_productTypeService.GetProductTypeByIdAsync(productTypeId));

        /// <summary>
        /// Get all product types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<IEnumerable<ProductType>>))]
        public async Task<JsonResult> GetProductTypes()
            => await JsonAsync(_productTypeService.GetProductTypesAsync());

        /// <summary>
        /// Create product type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [JsonProduces(typeof(ResultModel<Guid>))]
        public async Task<JsonResult> CreateProductType(CreateProductTypeViewModel model)
            => await JsonAsync(_productTypeService.AddProductTypeAsync(model));
    }
}