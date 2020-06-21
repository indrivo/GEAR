using GR.Core.Helpers;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;

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
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateUserAsync(TUser user, string password);

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
        string GetRequestIpAddress();

        /// <summary>
        /// Add roles to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel> AddToRolesAsync(TUser user, ICollection<string> roles);

        /// <summary>
        /// Add default roles
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel> AddDefaultRoles(GearUser user);

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IEnumerable<GearRole>> GetUserRolesAsync(TUser user);

        /// <summary>
        /// Disable user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> DisableUserAsync(Guid? userId);

        /// <summary>
        /// Enable user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> EnableUserAsync(Guid? userId);

        /// <summary>
        /// Set editable status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="editableStatus"></param>
        /// <returns></returns>
        Task<ResultModel> SetEditableStatusForUserAsync(Guid? userId, bool editableStatus);

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

        Task<ResultModel<IEnumerable<UserInfoViewModel>>> GetUsersInRoleForCurrentCompanyAsync([Required] string roleName);

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
        Task<ResultModel<IEnumerable<TUser>>> GetUsersInRolesAsync(IEnumerable<GearRole> roles, Guid? tenantId = null);

        /// <summary>
        /// Find roles by names
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<GearRole>>> FindRolesByNamesAsync(IEnumerable<string> roles);

        /// <summary>
        /// Find user by phone number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<TUser> FindByPhoneNumberAsync(string phone);

        /// <summary>
        /// Change user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeUserRolesAsync(Guid? userId, IEnumerable<Guid> roles);

        /// <summary>
        /// Delete user permanently
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteUserPermanently(Guid? userId);

        /// <summary>
        /// Check if current user has this id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsCurrentUser(Guid id);

        /// <summary>
        /// Get user image as bytes
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<byte[]>> GetUserImageAsync(Guid? userId);

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<UserListItemViewModel>> GetAllUsersWithPaginationAsync(DTParameters parameters);

        /// <summary>
        /// Find user id in claims
        /// </summary>
        /// <returns></returns>
        ResultModel<Guid> FindUserIdInClaims();

        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<GearUser>> FindUserByIdAsync(Guid? userId);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteUserAsync(Guid? userId);

        /// <summary>
        /// Restore user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel> RestoreUserAsync(Guid? userId);

        /// <summary>
        /// Remove user photo
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> RemoveUserPhotoAsync();

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Task<ResultModel> ChangeUserPasswordAsync(string current, string next);
    }
}