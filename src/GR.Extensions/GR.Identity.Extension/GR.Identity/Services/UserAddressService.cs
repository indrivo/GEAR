using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Services
{
    public class UserAddressService : IUserAddressService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IIdentityContext _context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion Injectable

        public UserAddressService(IIdentityContext context, IUserManager<GearUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get addresses for current user
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync()
        {
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<IEnumerable<Address>>(null);
            return await GetUserAddressesAsync(currentUserRequest.Result.Id.ToGuid());
        }

        /// <summary>
        /// Get addresses by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync(Guid? userId)
        {
            if (userId == null) return new InvalidParametersResultModel<IEnumerable<Address>>();
            var data = await _context.Addresses.Where(x => x.ApplicationUserId.ToGuid() == userId)
                .Include(x => x.ApplicationUser)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince)
                .ToListAsync();

            return new ResultModel<IEnumerable<Address>>
            {
                IsSuccess = true,
                Result = data
            };
        }

        /// <summary>
        /// Get countries
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Country>>> GetCountriesAsync()
        {
            var countrySelectList = await _context.Countries
                .AsNoTracking().ToListAsync();
            return new ResultModel<IEnumerable<Country>>
            {
                IsSuccess = true,
                Result = countrySelectList
            };
        }

        /// <summary>
        /// Get cities by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<StateOrProvince>>> GetCitiesByCountryIdAsync(string countryId)
        {
            if (countryId.IsNullOrEmpty()) return new InvalidParametersResultModel<IEnumerable<StateOrProvince>>();
            var data = await _context.StateOrProvinces.Where(x => x.CountryId.Equals(countryId)).ToListAsync();
            return new SuccessResultModel<IEnumerable<StateOrProvince>>(data);
        }

        /// <summary>
        /// Get address by id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Address>> GetAddressByIdAsync(Guid? addressId)
        {
            if (addressId == null) return new InvalidParametersResultModel<Address>();
            var address = await _context.Addresses
                .Include(x => x.ApplicationUser)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince)
                .FirstOrDefaultAsync(x => x.Id.Equals(addressId));

            if (address == null) return new NotFoundResultModel<Address>();
            return new SuccessResultModel<Address>(address);
        }

        /// <summary>
        /// Delete address
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteAddressAsync(Guid? id)
        {
            var resultModel = new ResultModel();
            if (!id.HasValue) return new InvalidParametersResultModel<object>().ToBase();

            var currentAddress = await _context.Addresses.FindAsync(id.Value);
            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return resultModel;
            }
            _context.Addresses.Remove(currentAddress);
            var dbResult = await _context.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Add new address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> AddAddressAsync(AddNewAddressViewModel model)
        {
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map(Guid.Empty);
            var user = currentUserRequest.Result;
            var address = new Address
            {
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                ContactName = model.ContactName,
                ZipCode = model.ZipCode,
                Phone = model.Phone,
                CountryId = model.CountryId,
                StateOrProvinceId = model.CityId,
                ApplicationUserId = user.Id,
                IsDefault = model.IsDefault
            };

            if (model.IsDefault)
            {
                _context.Addresses
                    .Where(x => x.ApplicationUserId.Equals(user.Id))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            await _context.Addresses.AddAsync(address);
            var dbResult = await _context.PushAsync();
            return dbResult.Map(address.Id);
        }
    }
}