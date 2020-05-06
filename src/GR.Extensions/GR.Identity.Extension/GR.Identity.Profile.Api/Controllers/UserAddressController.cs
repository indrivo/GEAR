using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Profile.Api.Controllers
{
    [JsonApiExceptionFilter]
    [GearAuthorize(GearAuthenticationScheme.Bearer | GearAuthenticationScheme.Identity)]
    public class UserAddressController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user address service
        /// </summary>
        private readonly IUserAddressService _addressService;

        #endregion Injectable

        public UserAddressController(IUserAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Get user addresses
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(DefaultApiRouteTemplate)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<Address>>))]
        public async Task<JsonResult> GetUserAddresses()
            => await JsonAsync(_addressService.GetUserAddressesAsync());

        /// <summary>
        /// Get default user address
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route(DefaultApiRouteTemplate)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<GetUserAddressViewModel>))]
        public async Task<JsonResult> GetDefaultUserAddress()
            => await JsonAsync(_addressService.GetDefaultAddressAsync());

        /// <summary>
        /// Add new address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route(DefaultApiRouteTemplate)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<Guid>))]
        public async Task<JsonResult> AddNewAddress(AddNewAddressViewModel model)
        {
            return !ModelState.IsValid ? Json(new ResultModel<Guid>().AttachModelState(ModelState))
                : Json(await _addressService.AddAddressAsync(model));
        }

        /// <summary>
        /// Delete address
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [HttpDelete, Route(DefaultApiRouteTemplate)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteAddress(Guid? addressId) => Json(await _addressService.DeleteAddressAsync(addressId));

        /// <summary>
        /// Update address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route(DefaultApiRouteTemplate)]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public virtual async Task<JsonResult> UpdateUserAddress(EditUserProfileAddressViewModel model)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            return await JsonAsync(_addressService.UpdateUserAddressAsync(model));
        }
    }
}