using GR.Core.Helpers;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GR.Identity.Abstractions
{
    public interface IUserManager<TUser> where TUser : GearUser
    {
        /// <summary>
        /// User manager
        /// </summary>
        UserManager<TUser> UserManager { get; }

        /// <summary>
        /// Role manager
        /// </summary>
        RoleManager<GearRole> RoleManager { get; }

        /// <summary>
        /// Identity context
        /// </summary>
        IIdentityContext IdentityContext { get; }

        /// <summary>
        /// Get the tenant id of current user
        /// </summary>
        Guid? CurrentUserTenantId { get; }

        /// <summary>
        /// Get the current user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<GearUser>> GetCurrentUserAsync();

        /// <summary>
        /// Get roles from claims
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetRolesFromClaims();

        /// <summary>
        /// Get request ip address
        /// </summary>
        /// <returns></returns>
        string GetRequestIpAdress();

        /// <summary>
        /// Add roles to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel> AddToRolesAsync(GearUser user, ICollection<string> roles);

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IEnumerable<GearRole>> GetUserRolesAsync(GearUser user);

        /// <summary>
        /// Disable user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> DisableUserAsync(Guid? userId);

        /// <summary>
        /// Get user addresses
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync(Guid? userId);

        /// <summary>
        /// Filter valid roles
        /// </summary>
        /// <param name="rolesIds"></param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> FilterValidRolesAsync(IEnumerable<Guid> rolesIds);

        /// <summary>
        /// Get users in role for current logged user
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>

        Task<ResultModel<IEnumerable<SampleGetUserViewModel>>> GetUsersInRoleForCurrentCompanyAsync([Required]string roleName);

        /// <summary>
        /// Find roles by id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<GearRole>> FindRolesByIdAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Get users in roles
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GearUser>>> GetUsersInRolesAsync(IEnumerable<GearRole> roles, Guid? tenantId = null);

        /// <summary>
        /// Find roles by names
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GearRole>>> FindRolesByNamesAsync(IEnumerable<string> roles);

        /// <summary>
        /// Change user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeUserRolesAsync(Guid? userId, IEnumerable<Guid> roles);
    }
}