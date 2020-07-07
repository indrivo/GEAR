using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Api.Controllers
{
    /// <summary>
    /// This is a products api
    /// </summary>
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/product-attributes/[action]")]
    public class ProductAttributesApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject attribute service
        /// </summary>
        private readonly IProductAttributeService _attributeService;

        #endregion

        public ProductAttributesApiController(IProductAttributeService attributeService)
        {
            _attributeService = attributeService;
        }

        /// <summary>
        /// Get product attributes
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(IEnumerable<ProductAttributeItemViewModel>))]
        public async Task<JsonResult> GetProductAttributes(Guid productId)
        => await JsonAsync(_attributeService.GetProductAttributesAsync(productId));

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<FilterAttributeValuesViewModel>>))]
        public async Task<JsonResult> GetAttributeFilters()
            => await JsonAsync(_attributeService.GetAttributesForFiltersAsync());

        /// <summary>
        /// Edit product attributes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdateProductAttributes([FromBody] IEnumerable<ProductAttributesViewModel> model)
            => await JsonAsync(_attributeService.AddOrUpdateProductAttributesAsync(model));

        /// <summary>
        /// Remove attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> RemoveAttribute(Guid productId, Guid attributeId)
            => await JsonAsync(_attributeService.RemoveAttributeAsync(productId, attributeId));
    }
}