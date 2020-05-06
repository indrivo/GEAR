using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Countries;
using GR.Localization.Abstractions.ViewModels.CountryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Localization.Api.Controllers
{
    [JsonApiExceptionFilter]
    [GearAuthorize(GearAuthenticationScheme.Bearer | GearAuthenticationScheme.Identity)]
    [Route("api/country/[action]")]
    public class CountryApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject location service
        /// </summary>
        private readonly ICountryService _countryService;

        #endregion Injectable

        public CountryApiController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        /// <summary>
        /// Add new country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> AddNewCountry([Required]AddCountryViewModel model)
         => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _countryService.AddNewCountryAsync(model));

        /// <summary>
        /// Delete country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteCountry([Required]string countryId)
            => await JsonAsync(_countryService.DeleteCountryAsync(countryId));

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<AddCountryViewModel>>))]
        public async Task<JsonResult> GetAllCountries()
            => await JsonAsync(_countryService.GetAllCountriesAsync());

        /// <summary>
        /// Get country bu id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<Country>))]
        public async Task<JsonResult> GetCountryById([Required]string countryId)
            => await JsonAsync(_countryService.GetCountryByIdAsync(countryId));

        /// <summary>
        /// Update country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateCountry([Required]AddCountryViewModel model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _countryService.UpdateCountryAsync(model));

        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<StateOrProvince>>))]
        public async Task<JsonResult> GetCitiesByCountry([Required] string countryId)
        {
            var request = await _countryService.GetCitiesByCountryAsync(countryId);
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
            var request = await _countryService.AddCityToCountryAsync(model);
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
            => await JsonAsync(_countryService.RemoveCityAsync(cityId));

        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<StateOrProvince>))]
        public async Task<JsonResult> GetCityById([Required] Guid cityId)
            => await JsonAsync(_countryService.GetCityByIdAsync(cityId));

        /// <summary>
        /// Update city
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> UpdateCity([Required]StateOrProvince model)
            => !ModelState.IsValid ? Json(new ResultModel().AttachModelState(ModelState)) : Json(await _countryService.UpdateCityAsync(model));

        /// <summary>
        /// Get countries details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfo()
            => await JsonAsync(_countryService.GetCountriesInfoAsync());

        /// <summary>
        /// Search by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfoByName(string name)
            => await JsonAsync(_countryService.GetCountriesInfoByNameAsync(name));


        /// <summary>
        /// Search by iso code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfoByIsoCode(string code)
            => await JsonAsync(_countryService.GetCountriesInfoByIsoCodeAsync(code));

        /// <summary>
        /// Search by currency code
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfoByIsoCurrency(string currency)
            => await JsonAsync(_countryService.GetCountriesInfoByIsoCurrencyAsync(currency));


        /// <summary>
        /// Search by capital
        /// </summary>
        /// <param name="capital"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfoByCapital(string capital)
            => await JsonAsync(_countryService.GetCountriesInfoByCapitalCityAsync(capital));

        /// <summary>
        /// Search by calling code
        /// </summary>
        /// <param name="callingCode"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfoByCallingCode(string callingCode)
            => await JsonAsync(_countryService.GetCountriesInfoByCallingCodeAsync(callingCode));


        /// <summary>
        /// Search by region: Africa, Americas, Asia, Europe, Oceania
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<CountryInfoViewModel>>))]
        public async Task<JsonResult> GetCountriesInfoByRegion(string region)
            => await JsonAsync(_countryService.GetCountriesInfoByRegionAsync(region));
    }
}