using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile
{
    [Author(Authors.LUPEI_NICOLAE)]
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

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion Injectable

        public UserAddressService(IProfileContext context, IUserManager<GearUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Get addresses for current user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync()
        {
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<IEnumerable<Address>>();
            return await GetUserAddressesAsync(currentUserRequest.Result.Id);
        }

        /// <summary>
        /// Get default address
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<GetUserAddressViewModel>> GetDefaultAddressAsync()
        {
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<GetUserAddressViewModel>();
            var query = _context.UserAddresses.Where(x => x.UserId == currentUserRequest.Result.Id)
                .Include(x => x.User)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince);

            var defaultAddress = await query.FirstOrDefaultAsync(x => x.IsDefault)
                                 ?? await query.FirstOrDefaultAsync();

            if (defaultAddress == null)
            {
                return new NotFoundResultModel<GetUserAddressViewModel>();
            }

            var mapped = _mapper.Map<GetUserAddressViewModel>(defaultAddress);
            return new SuccessResultModel<GetUserAddressViewModel>(mapped);
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
        /// Update user address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateUserAddressAsync(EditUserProfileAddressViewModel model)
        {
            var resultModel = new ResultModel();
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            if (user == null) return new NotAuthorizedResultModel().ToBase();
            var currentAddress = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));

            if (currentAddress == null && model.Id != Guid.Empty)
            {
                if (!await _context.UserAddresses.AnyAsync(x => x.UserId == user.Id))
                {
                    var newAddress = _mapper.Map<AddNewAddressViewModel>(model);
                    newAddress.IsDefault = true;
                    var addReq = await AddAddressAsync(newAddress);
                    return addReq.ToBase();
                }
            }

            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return resultModel;
            }

            if (model.IsDefault)
            {
                _context.UserAddresses
                    .Where(x => x.UserId.Equals(currentAddress.UserId))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            currentAddress.CountryId = model.CountryId;
            currentAddress.StateOrProvinceId = model.CityId;
            currentAddress.AddressLine1 = model.AddressLine1;
            currentAddress.AddressLine2 = model.AddressLine2;
            currentAddress.ContactName = model.ContactName;
            currentAddress.Phone = model.Phone;
            currentAddress.ZipCode = model.ZipCode;
            currentAddress.IsDefault = model.IsDefault;
            currentAddress.Region = model.Region;

            _context.Update(currentAddress);
            var result = await _context.PushAsync();
            return result;
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
                IsDefault = model.IsDefault,
                Region = model.Region
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