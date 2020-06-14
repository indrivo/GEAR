using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Localization.Abstractions;
using GR.Localization.Abstractions.Models.Countries;
using GR.Localization.Abstractions.ViewModels.CountryViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GR.Localization
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Base implementation of country service")]
    public class CountryService : ICountryService
    {
        const string Key = "gear-country-info-key";

        #region Injectable

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly ICountryContext _context;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;
        #endregion 

        public CountryService(ICountryContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Add new country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddNewCountryAsync(AddCountryViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel();
            await _context.Countries.AddAsync(model.Adapt<Country>());
            return await _context.PushAsync();
        }

        /// <summary>
        /// Remove country
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteCountryAsync(string countryId)
        {
            if (countryId.IsNullOrEmpty()) return new InvalidParametersResultModel();
            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(countryId));
            if (country == null) return new NotFoundResultModel();
            _context.Countries.Remove(country);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<AddCountryViewModel>>> GetAllCountriesAsync()
        {
            var data = await _context.Countries
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
            var mapped = data.Adapt<IEnumerable<AddCountryViewModel>>();
            return new SuccessResultModel<IEnumerable<AddCountryViewModel>>(mapped);
        }

        /// <summary>
        /// Get country by id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Country>> GetCountryByIdAsync(string countryId)
        {
            if (countryId.IsNullOrEmpty()) return new InvalidParametersResultModel<Country>();
            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(countryId));
            if (country == null) return new NotFoundResultModel<Country>();
            return new SuccessResultModel<Country>(country);
        }

        /// <summary>
        /// Update country meta data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateCountryAsync(AddCountryViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel();
            var country = await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (country == null) return new NotFoundResultModel();
            country.Code3 = model.Code3;
            country.IsBillingEnabled = model.IsBillingEnabled;
            country.IsCityEnabled = model.IsCityEnabled;
            country.IsDistrictEnabled = model.IsDistrictEnabled;
            country.IsShippingEnabled = model.IsShippingEnabled;
            country.IsZipCodeEnabled = model.IsZipCodeEnabled;
            country.Name = model.Name;
            _context.Countries.Update(country);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="search"></param>
        /// <param name="selectedCityId"></param>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<StateOrProvince>>> GetCitiesByCountryAsync(string countryId, string search, Guid? selectedCityId, int maxItems = 20)
        {
            if (countryId.IsNullOrEmpty()) return new InvalidParametersResultModel<IEnumerable<StateOrProvince>>();
            var query = _context.StateOrProvinces
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Where(x => x.CountryId.Equals(countryId));
            if (!search.IsNullOrEmpty())
            {
                query = query.Where(x => x.Name.ToLowerInvariant().StartsWith(search.ToLowerInvariant()));
            }

            query = query.Take(maxItems);

            var data = await query.ToListAsync();
            if (selectedCityId == null || data.Select(x => x.Id).Contains(selectedCityId.Value))
                return new SuccessResultModel<IEnumerable<StateOrProvince>>(data);

            var selected = await _context.StateOrProvinces
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(selectedCityId) && x.CountryId.Equals(countryId));
            if (selected != null)
            {
                data = data.Prepend(selected).ToList();
            }

            return new SuccessResultModel<IEnumerable<StateOrProvince>>(data);
        }

        /// <summary>
        /// Add city to country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddCityToCountryAsync(AddCityViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel<Guid>();
            var id = Guid.NewGuid();
            var dataModel = new StateOrProvince(id)
            {
                Name = model.Name,
                Code = model.Code,
                CountryId = model.CountryId,
                Type = model.Type
            };
            await _context.StateOrProvinces.AddAsync(dataModel);
            var dbRequest = await _context.PushAsync();
            return dbRequest.Map(dataModel.Id);
        }

        /// <summary>
        /// Remove city
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveCityAsync(Guid cityId)
        {
            var city = await _context.StateOrProvinces
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(cityId));
            if (city == null) return new NotFoundResultModel();
            _context.StateOrProvinces.Remove(city);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<StateOrProvince>> GetCityByIdAsync(Guid cityId)
        {
            var city = await _context.StateOrProvinces
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(cityId));
            if (city == null) return new NotFoundResultModel<StateOrProvince>();
            return new SuccessResultModel<StateOrProvince>(city);
        }

        /// <summary>
        /// Update city
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateCityAsync(StateOrProvince model)
        {
            if (model == null) return new InvalidParametersResultModel();
            var city = await _context.StateOrProvinces
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (city == null) return new NotFoundResultModel();
            city.Name = model.Name;
            city.CountryId = model.CountryId;
            city.Type = model.Type;
            _context.StateOrProvinces
                .Update(city);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get country info
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoAsync()
        {
            var cacheData = await _cacheService.GetAsync<List<CountryInfoViewModel>>(Key);
            if (cacheData?.Any() ?? false) return new SuccessResultModel<ICollection<CountryInfoViewModel>>(cacheData);
            var path = Path.Combine(AppContext.BaseDirectory, "Configuration/countries-source.json");
            var data = JsonParser.ReadArrayDataFromJsonFile<ICollection<CountryInfoViewModel>>(path)?.ToList();
            await _cacheService.SetAsync(Key, data);
            return new SuccessResultModel<ICollection<CountryInfoViewModel>>(data);
        }

        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByNameAsync(string name)
        {
            var request = await GetCountriesInfoAsync();
            if (!request.IsSuccess) return request;
            request.Result = request.Result.Where(x => x.Name.Contains(name))
                .ToList();
            return request;
        }

        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByIsoCodeAsync(string code)
        {
            var request = await GetCountriesInfoAsync();
            if (!request.IsSuccess) return request;
            request.Result = request.Result.Where(x => x.Alpha2Code == code || x.Alpha3Code == code)
                .ToList();
            return request;
        }

        /// <summary>
        /// Search by ISO 4217 currency code
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByIsoCurrencyAsync(string currency)
        {
            var request = await GetCountriesInfoAsync();
            if (!request.IsSuccess) return request;
            request.Result = request.Result.Where(x => x.Currencies.Any(y => y.Code.Equals(currency)))
                .ToList();
            return request;
        }

        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByCapitalCityAsync(string capital)
        {
            var request = await GetCountriesInfoAsync();
            if (!request.IsSuccess) return request;
            request.Result = request.Result.Where(x => x.Capital.Equals(capital))
                .ToList();
            return request;
        }

        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByCallingCodeAsync(string callingCode)
        {
            var request = await GetCountriesInfoAsync();
            if (!request.IsSuccess) return request;
            request.Result = request.Result.Where(x => x.CallingCodes.Any(c => c == callingCode))
                .ToList();
            return request;
        }

        /// <summary>
        /// Search by region: Africa, Americas, Asia, Europe, Oceania
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<ICollection<CountryInfoViewModel>>> GetCountriesInfoByRegionAsync(string region)
        {
            var request = await GetCountriesInfoAsync();
            if (!request.IsSuccess) return request;
            request.Result = request.Result.Where(x => x.Region.Equals(region))
                .ToList();
            return request;
        }

        /// <summary>
        /// Import countries
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ImportCountriesAsync(IEnumerable<Country> countries)
        {
            foreach (var country in countries)
            {
                if (await _context.Countries.AnyAsync(x => x.Id.Equals(country.Id)))
                {
                    var existCities = await _context.StateOrProvinces
                        .AsNoTracking()
                        .Where(x => x.CountryId.Equals(country.Id))
                        .ToListAsync();
                    var newCities = country.StatesOrProvinces
                        .Where(x => !existCities.Select(y => y.Id).Contains(x.Id))
                        .Select(x => new StateOrProvince
                        {
                            Id = x.Id,
                            Code = x.Code,
                            CountryId = x.CountryId,
                            Name = x.Name,
                            Type = x.Type,
                            DisableAuditTracking = true
                        })
                        .ToList();
                    if (newCities.Any()) await _context.StateOrProvinces.AddRangeAsync(newCities);
                }
                else
                {
                    await _context.Countries.AddAsync(country);
                }
                await _context.PushAsync();
            }

            return new SuccessResultModel<object>().ToBase();
        }
    }
}