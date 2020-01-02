using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.LocationViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Services
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Base implementation of location service")]
    public class LocationService : ILocationService
    {
        #region Injectable

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IIdentityContext _context;

        #endregion Injectable

        public LocationService(IIdentityContext context)
        {
            _context = context;
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
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Id.Equals(countryId));
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
            var data = await _context.Countries.ToListAsync();
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
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Id.Equals(countryId));
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
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<StateOrProvince>>> GetCitiesByCountryAsync(string countryId)
        {
            if (countryId.IsNullOrEmpty()) return new InvalidParametersResultModel<IEnumerable<StateOrProvince>>();
            var data = await _context.StateOrProvinces.Where(x => x.CountryId.Equals(countryId)).ToListAsync();
            return new SuccessResultModel<IEnumerable<StateOrProvince>>(data);
        }

        /// <summary>
        /// Add city to country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<long>> AddCityToCountryAsync(AddCityViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel<long>();
            var cities = await _context.StateOrProvinces.Select(x => x.Id).ToListAsync();
            var id = cities.GenerateUniqueNumberThatNoIncludesNumbers();
            var dataModel = model.Adapt<StateOrProvince>();
            dataModel.Id = id;
            await _context.StateOrProvinces.AddAsync(dataModel);
            var dbRequest = await _context.PushAsync();
            return dbRequest.Map(dataModel.Id);
        }

        /// <summary>
        /// Remove city
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveCityAsync(long cityId)
        {
            var city = await _context.StateOrProvinces.FirstOrDefaultAsync(x => x.Id.Equals(cityId));
            if (city == null) return new NotFoundResultModel();
            _context.StateOrProvinces.Remove(city);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Get city by id
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<StateOrProvince>> GetCityByIdAsync(long cityId)
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
            _context.StateOrProvinces.Update(city);
            return await _context.PushAsync();
        }
    }
}