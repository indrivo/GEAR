using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.LocationViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Razor.Controllers
{
    public class LocationController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject location service
        /// </summary>
        private readonly ILocationService _locationService;

        #endregion

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Add new country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> AddNewCountry([Required]AddCountryViewModel model)
         => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.AddNewCountryAsync(model));


        /// <summary>
        /// Delete country 
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteCountry([Required]string countryId)
            => await JsonAsync(_locationService.DeleteCountryAsync(countryId));


        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<AddCountryViewModel>>))]
        public async Task<JsonResult> GetAllCountries()
            => await JsonAsync(_locationService.GetAllCountriesAsync());

        /// <summary>
        /// Get country bu id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<Country>))]
        public async Task<JsonResult> GetCountryById([Required]string countryId)
            => await JsonAsync(_locationService.GetCountryByIdAsync(countryId));


        /// <summary>
        /// Update country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateCountry([Required]AddCountryViewModel model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.UpdateCountryAsync(model));


        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<StateOrProvince>>))]
        public async Task<JsonResult> GetCitiesByCountry([Required] string countryId)
            => await JsonAsync(_locationService.GetCitiesByCountryAsync(countryId));


        /// <summary>
        /// Add city to country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<long>))]
        public async Task<JsonResult> AddCityToCountry([Required]AddCityViewModel model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.AddCityToCountryAsync(model));


        /// <summary>
        /// Remove city bu id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveCity([Required] long cityId)
            => await JsonAsync(_locationService.RemoveCityAsync(cityId));


        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<StateOrProvince>))]
        public async Task<JsonResult> GetCityById([Required] long cityId)
            => await JsonAsync(_locationService.GetCityByIdAsync(cityId));

        /// <summary>
        /// Update city
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateCity([Required]StateOrProvince model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.UpdateCityAsync(model));
    }
}
