using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Countries;
using GR.Localization.Abstractions.ViewModels.CountryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Localization.Api.Controllers
{
    [Authorize]
    [Roles(GlobalResources.Roles.ADMINISTRATOR)]
    [Route("api/country/[action]")]
    public class CountryApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject location service
        /// </summary>
        private readonly ICountryService _locationService;

        #endregion Injectable

        public CountryApiController(ICountryService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Add new country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> AddNewCountry([Required]AddCountryViewModel model)
         => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.AddNewCountryAsync(model));

        /// <summary>
        /// Delete country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteCountry([Required]string countryId)
            => await JsonAsync(_locationService.DeleteCountryAsync(countryId));

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<AddCountryViewModel>>))]
        public async Task<JsonResult> GetAllCountries()
            => await JsonAsync(_locationService.GetAllCountriesAsync());

        /// <summary>
        /// Get country bu id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<Country>))]
        public async Task<JsonResult> GetCountryById([Required]string countryId)
            => await JsonAsync(_locationService.GetCountryByIdAsync(countryId));

        /// <summary>
        /// Update country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateCountry([Required]AddCountryViewModel model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.UpdateCountryAsync(model));

        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<StateOrProvince>>))]
        public async Task<JsonResult> GetCitiesByCountry([Required] string countryId)
        {
            var request = await _locationService.GetCitiesByCountryAsync(countryId);
            if (!request.IsSuccess) return Json(request);
            var vm = request.Result.Select(x => new
            {
                Id = x.Id.ToString(),
                x.Name,
                x.CountryId,
                x.Type,
                x.Code
            });
            return Json(new SuccessResultModel<IEnumerable<object>>(vm));
        }

        /// <summary>
        /// Add city to country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<string>))]
        public async Task<JsonResult> AddCityToCountry([Required] AddCityViewModel model)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            var request = await _locationService.AddCityToCountryAsync(model);
            return Json(request.Map(request.Result.ToString()));
        }

        /// <summary>
        /// Remove city bu id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> RemoveCity([Required] Guid cityId)
            => await JsonAsync(_locationService.RemoveCityAsync(cityId));

        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<StateOrProvince>))]
        public async Task<JsonResult> GetCityById([Required] Guid cityId)
            => await JsonAsync(_locationService.GetCityByIdAsync(cityId));

        /// <summary>
        /// Update city
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateCity([Required]StateOrProvince model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _locationService.UpdateCityAsync(model));
    }
}