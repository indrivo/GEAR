using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Localization.Abstractions.Models.Countries;
using GR.Localization.Abstractions.ViewModels.CountryViewModels;

namespace GR.Localization.Abstractions
{
    public interface ICountryService
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
        Task<ResultModel<Guid>> AddCityToCountryAsync(AddCityViewModel model);

        /// <summary>
        /// Remove city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveCityAsync(Guid cityId);

        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        Task<ResultModel<StateOrProvince>> GetCityByIdAsync(Guid cityId);

        /// <summary>
        /// Update city
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateCityAsync(StateOrProvince model);

        /// <summary>
        /// Get countries info
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoAsync();

        /// <summary>
        /// Search by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByNameAsync(string name);

        /// <summary>
        /// Search by iso code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByIsoCodeAsync(string code);

        /// <summary>
        /// Search by currency
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByIsoCurrencyAsync(string currency);

        /// <summary>
        /// Search by capital
        /// </summary>
        /// <param name="capital"></param>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByCapitalCityAsync(string capital);

        /// <summary>
        /// Search by calling code
        /// </summary>
        /// <param name="callingCode"></param>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByCallingCodeAsync(string callingCode);

        /// <summary>
        /// Search by region: Africa, Americas, Asia, Europe, Oceania
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByRegionAsync(string region);
    }
}