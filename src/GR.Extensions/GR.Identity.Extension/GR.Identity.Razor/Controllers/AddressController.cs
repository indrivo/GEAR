using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR.Identity.Razor.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject user address service
        /// </summary>
        private readonly IUserAddressService _addressService;

        #endregion Injectable

        public AddressController(IUserAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]"), AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<Country>>))]
        public async Task<JsonResult> GetAllCountries() => Json(await _addressService.GetCountriesAsync());

        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]"), AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<Country>>))]
        public async Task<JsonResult> GetCitiesByCountryId(string countryId) => Json(await _addressService.GetCitiesByCountryIdAsync(countryId));

        /// <summary>
        /// Get user addresses
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<Address>>))]
        public async Task<JsonResult> GetUserAddresses() => Json(await _addressService.GetUserAddressesAsync());

        /// <summary>
        /// Add new address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
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
        [HttpDelete, Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteAddress(Guid? addressId) => Json(await _addressService.DeleteAddressAsync(addressId));
    }
}