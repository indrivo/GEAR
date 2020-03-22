using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using GR.Identity.Profile.Abstractions.Models.AddressModels;

namespace GR.Identity.Profile.Abstractions
{
    public interface IUserAddressService
    {
        /// <summary>
        /// Get addresses for current user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync();

        /// <summary>
        /// Get addresses for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync(Guid? userId);

        /// <summary>
        /// Delete address
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteAddressAsync(Guid? id);

        /// <summary>
        /// Add new address for current user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddAddressAsync(AddNewAddressViewModel model);

        /// <summary>
        /// Get address by id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        Task<ResultModel<Address>> GetAddressByIdAsync(Guid? addressId);
    }
}