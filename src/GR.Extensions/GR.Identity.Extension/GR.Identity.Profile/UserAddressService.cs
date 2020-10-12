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
        protected readonly IProfileContext Context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        protected readonly IUserManager<GearUser> UserManager;

        /// <summary>
        /// Inject mapper
        /// </summary>
        protected readonly IMapper Mapper;

        #endregion Injectable

        public UserAddressService(IProfileContext context, IUserManager<GearUser> userManager, IMapper mapper)
        {
            Context = context;
            UserManager = userManager;
            Mapper = mapper;
        }

        /// <summary>
        /// Get addresses for current user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync()
        {
            var currentUserRequest = UserManager.FindUserIdInClaims();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<IEnumerable<Address>>();
            return await GetUserAddressesAsync(currentUserRequest.Result);
        }

        /// <summary>
        /// Get default address
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<GetUserAddressViewModel>> GetDefaultAddressAsync()
        {
            var currentUserRequest = UserManager.FindUserIdInClaims();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<GetUserAddressViewModel>();
            return await GetDefaultAddressAsync(currentUserRequest.Result);
        }

        /// <summary>
        /// Get default address of user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<GetUserAddressViewModel>> GetDefaultAddressAsync(Guid? userId)
        {
            var query = Context.UserAddresses.Where(x => x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince);

            var defaultAddress = await query.FirstOrDefaultAsync(x => x.IsDefault)
                                 ?? await query.FirstOrDefaultAsync();

            if (defaultAddress == null)
            {
                return new NotFoundResultModel<GetUserAddressViewModel>();
            }

            var mapped = Mapper.Map<GetUserAddressViewModel>(defaultAddress);
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
            var data = await Context.UserAddresses.Where(x => x.UserId == userId)
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
            var address = await Context.UserAddresses
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
            var user = (await UserManager.GetCurrentUserAsync()).Result;
            if (user == null) return new NotAuthorizedResultModel().ToBase();
            var currentAddress = await Context.UserAddresses.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));

            if (currentAddress == null && model.Id != Guid.Empty)
            {
                if (!await Context.UserAddresses.AnyAsync(x => x.UserId == user.Id))
                {
                    var newAddress = Mapper.Map<AddNewAddressViewModel>(model);
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
                Context.UserAddresses
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

            Context.Update(currentAddress);
            var result = await Context.PushAsync();
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

            var currentAddress = await Context.UserAddresses.FindAsync(id.Value);
            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return resultModel;
            }
            Context.UserAddresses.Remove(currentAddress);
            var dbResult = await Context.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Add new address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddAddressAsync(AddNewAddressViewModel model)
        {
            var currentUserRequest = await UserManager.GetCurrentUserAsync();
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
                Context.UserAddresses
                    .Where(x => x.UserId.Equals(user.Id))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            await Context.UserAddresses.AddAsync(address);
            var dbResult = await Context.PushAsync();
            return dbResult.Map(address.Id);
        }
    }
}