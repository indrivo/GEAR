using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile
{
    public class UserAddressService : IUserAddressService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IProfileContext _context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion Injectable

        public UserAddressService(IProfileContext context, IUserManager<GearUser> userManager)
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
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<IEnumerable<Address>>();
            return await GetUserAddressesAsync(currentUserRequest.Result.Id);
        }

        /// <summary>
        /// Get addresses by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync(Guid? userId)
        {
            if (userId == null) return new InvalidParametersResultModel<IEnumerable<Address>>();
            var data = await _context.UserAddresses.Where(x => x.UserId == userId)
                .Include(x => x.User)
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
        /// Get address by id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Address>> GetAddressByIdAsync(Guid? addressId)
        {
            if (addressId == null) return new InvalidParametersResultModel<Address>();
            var address = await _context.UserAddresses
                .Include(x => x.User)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince)
                .AsNoTracking()
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

            var currentAddress = await _context.UserAddresses.FindAsync(id.Value);
            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return resultModel;
            }
            _context.UserAddresses.Remove(currentAddress);
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
                UserId = user.Id,
                IsDefault = model.IsDefault
            };

            if (model.IsDefault)
            {
                _context.UserAddresses
                    .Where(x => x.UserId.Equals(user.Id))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            await _context.UserAddresses.AddAsync(address);
            var dbResult = await _context.PushAsync();
            return dbResult.Map(address.Id);
        }
    }
}