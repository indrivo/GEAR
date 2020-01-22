using GR.Core.Helpers;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.LocationViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR.Identity.Abstractions
{
    public interface ILocationService
    {
        /// <summary>
        /// Add new country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddNewCountryAsync(AddCountryViewModel model);

        /// <summary>
        /// Remove country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteCountryAsync(string countryId);

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<AddCountryViewModel>>> GetAllCountriesAsync();

        /// <summary>
        /// Get country by id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        Task<ResultModel<Country>> GetCountryByIdAsync(string countryId);

        /// <summary>
        /// Update country metadata
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateCountryAsync(AddCountryViewModel model);

        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<StateOrProvince>>> GetCitiesByCountryAsync(string countryId);

        /// <summary>
        /// Add city to country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<long>> AddCityToCountryAsync(AddCityViewModel model);

        /// <summary>
        /// Remove city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveCityAsync(long cityId);

        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        Task<ResultModel<StateOrProvince>> GetCityByIdAsync(long cityId);

        /// <summary>
        /// Update city
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateCityAsync(StateOrProvince model);
    }
}