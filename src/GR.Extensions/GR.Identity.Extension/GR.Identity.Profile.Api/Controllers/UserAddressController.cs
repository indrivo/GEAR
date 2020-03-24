using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Profile.Api.Controllers
{
    [Authorize]
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
        public async Task<JsonResult> GetUserAddresses() => Json(await _addressService.GetUserAddressesAsync());

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