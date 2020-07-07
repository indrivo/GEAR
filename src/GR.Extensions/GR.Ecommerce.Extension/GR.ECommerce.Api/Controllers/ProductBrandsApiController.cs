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
using GR.ECommerce.Abstractions.Helpers.PermissionConfigurations;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.BrandViewModels;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Permissions.Abstractions.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Api.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route("api/product-brands/[action]")]
    public class ProductBrandsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject brands service
        /// </summary>
        private readonly IBrandsService _brandsService;

        #endregion

        public ProductBrandsApiController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }

        /// <summary>
        /// Get brands
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(BrandPermissions.ReadBrand)]
        [JsonProduces(typeof(ResultModel<IEnumerable<Brand>>))]
        public async Task<JsonResult> GetBrands()
            => await JsonAsync(_brandsService.GetBrandsAsync());

        /// <summary>
        /// Get brands with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(BrandPermissions.ReadBrand)]
        [JsonProduces(typeof(ResultModel<DTResult<Brand>>))]
        public async Task<JsonResult> GetBrandsWithPagination(DTParameters parameters)
            => await JsonAsync(_brandsService.GetBrandsWithPaginationAsync(parameters));

        /// <summary>
        /// Create new brand
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AuthorizePermission(BrandPermissions.CreateBrand)]
        [JsonProduces(typeof(ResultModel<Guid>))]
        public async Task<JsonResult> CreateNewBrand([Required] CreateBrandViewModel model)
            => await JsonAsync(_brandsService.CreateNewBrandAsync(model));

        /// <summary>
        /// Update brand
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(BrandPermissions.EditBrand)]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> UpdateBrand([Required] UpdateBrandViewModel model)
            => await JsonAsync(_brandsService.UpdateBrandAsync(model));
    }
}