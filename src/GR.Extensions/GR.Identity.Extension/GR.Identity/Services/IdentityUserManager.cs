using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Users;

namespace GR.Identity.Services
{
    [Author(Authors.LUPEI_NICOLAE)]
    [Documentation("Base implementation of gear user manager")]
    public class IdentityUserManager : IUserManager<GearUser>
    {
        #region Helpers

        /// <summary>
        /// Default roles
        /// </summary>
        protected readonly Collection<string> DefaultRoles = new Collection<string> {
            GlobalResources.Roles.USER,
            GlobalResources.Roles.ANONIMOUS_USER
        };

        #endregion

        #region Injectable

        /// <inheritdoc />
        /// <summary>
        /// Inject user manager
        /// </summary>
        public UserManager<GearUser> UserManager { get; }

        /// <inheritdoc />
        /// <summary>
        /// Inject role manager
        /// </summary>
        public RoleManager<GearRole> RoleManager { get; }

        /// <inheritdoc />
        /// <summary>
        /// Identity context
        /// </summary>
        public IIdentityContext IdentityContext { get; }

        /// <summary>
        /// Inject context accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        public IdentityUserManager(UserManager<GearUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<GearRole> roleManager, IIdentityContext identityContext)
        {
            UserManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            RoleManager = roleManager;
            IdentityContext = identityContext;
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> CreateUserAsync(GearUser user, string password)
        {
            if (user == null) return new InvalidParametersResultModel<Guid>();
            var createResponse = await UserManager.CreateAsync(user, password);
            var response = createResponse.ToResultModel<Guid>();
            response.Result = user.Id;
            if (response.IsSuccess)
            {
                await AddDefaultRoles(user);
            }
            return response;
        }


        /// <inheritdoc />
        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<GearUser>> GetCurrentUserAsync()
        {
            var result = new ResultModel<GearUser>();
            if (_httpContextAccessor.HttpContext == null)
            {
                result.Errors.Add(new ErrorModel("", "Unauthorized user"));
                return result;
            }
            var user = await UserManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            result.IsSuccess = user != null;
            result.Result = user;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get roles from claims
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetRolesFromClaims()
        {
            var roles = _httpContextAccessor.HttpContext.User.Claims
                .Where(x => x.Type.Equals("role") || x.Type.EndsWith("role")).Select(x => x.Value)
                .ToList();
            return roles;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get request ip address
        /// </summary>
        /// <returns></returns>
        public virtual string GetRequestIpAdress() => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();

        /// <inheritdoc />
        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual Guid? CurrentUserTenantId
        {
            get
            {
                Guid? val = _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value
                                ?.ToGuid() ?? GearSettings.TenantId;
                var userManager = IoC.Resolve<UserManager<GearUser>>();
                if (val != Guid.Empty) return val;
                var user = userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User).GetAwaiter()
                    .GetResult();
                if (user == null) return null;
                var userClaims = userManager.GetClaimsAsync(user).GetAwaiter().GetResult();
                val = userClaims?.FirstOrDefault(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value))?.Value?.ToGuid();

                return val;
            }
        }

        /// <summary>
        /// Add default roles
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddDefaultRoles(GearUser user) => await AddToRolesAsync(user, DefaultRoles);

        /// <inheritdoc />
        /// <summary>
        /// Add roles to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddToRolesAsync(GearUser user, ICollection<string> roles)
        {
            var result = new ResultModel();

            if (user == null || roles == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Bad parameters"));
                return result;
            }

            var exist = await UserManager.FindByNameAsync(user.UserName);
            if (exist == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "User not found"));
                return result;
            }

            foreach (var defaultRole in DefaultRoles)
            {
                if (roles.Contains(defaultRole)) continue;
                roles.Add(defaultRole);
            }

            var existentRoles = await UserManager.GetRolesAsync(exist);

            var newRoles = roles.Where(x => !existentRoles.Contains(x)).ToList();

            var serviceResult = await UserManager.AddToRolesAsync(exist, newRoles);

            if (serviceResult.Succeeded)
            {
                result.IsSuccess = true;
            }
            else result.AppendIdentityErrors(serviceResult.Errors);

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<GearRole>> GetUserRolesAsync(GearUser user)
        {
            if (user == null) throw new NullReferenceException();
            var roles = await UserManager.GetRolesAsync(user);
            return roles.Select(async x => await RoleManager.FindByNameAsync(x)).Select(x => x.Result);
        }

        /// <summary>
        /// Disable user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DisableUserAsync(Guid? userId)
        {
            var response = new ResultModel();
            if (userId == null) return response;
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (user == null) return response;
            if (CurrentUserTenantId != user.TenantId) return response;
            user.IsDisabled = true;
            var request = await UserManager.UpdateAsync(user);
            return request.ToResultModel<object>().ToBase();
        }

        /// <summary>
        /// Enable user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> EnableUserAsync(Guid? userId)
        {
            var response = new ResultModel();
            if (userId == null) return response;
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (user == null) return response;
            if (CurrentUserTenantId != user.TenantId) return response;
            user.IsDisabled = false;
            var request = await UserManager.UpdateAsync(user);
            return request.ToResultModel<object>().ToBase();
        }

        /// <summary>
        /// Set editable status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="editableStatus"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetEditableStatusForUserAsync(Guid? userId, bool editableStatus)
        {
            var response = new ResultModel();
            if (userId == null) return response;
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (user == null) return response;
            if (CurrentUserTenantId != user.TenantId) return response;
            user.IsEditable = editableStatus;
            var request = await UserManager.UpdateAsync(user);
            return request.ToResultModel<object>().ToBase();
        }

        /// <summary>
        /// Get user addresses
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Address>>> GetUserAddressesAsync(Guid? userId)
        {
            if (userId == null) return new NotFoundResultModel<IEnumerable<Address>>();
            var addresses = await IdentityContext.Addresses
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince)
                .Where(x => x.ApplicationUserId.Equals(userId))
                .NonDeleted()
                .ToListAsync();
            return new ResultModel<IEnumerable<Address>>
            {
                IsSuccess = true,
                Result = addresses
            };
        }

        /// <summary>
        /// Filter valid roles
        /// </summary>
        /// <param name="rolesIds"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Guid>> FilterValidRolesAsync(IEnumerable<Guid> rolesIds)
        {
            var data = new List<Guid>();
            foreach (var roleId in rolesIds)
            {
                var role = await RoleManager.FindByIdAsync(roleId.ToString());
                if (role != null) data.Add(roleId);
            }

            return data;
        }

        /// <summary>
        /// Get users in role for current logged user
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<SampleGetUserViewModel>>> GetUsersInRoleForCurrentCompanyAsync([Required]string roleName)
        {
            if (roleName.IsNullOrEmpty()) return new InvalidParametersResultModel<IEnumerable<SampleGetUserViewModel>>();
            var currentUserRequest = await GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<IEnumerable<SampleGetUserViewModel>>();
            var allUsers = await UserManager.GetUsersInRoleAsync(roleName);
            var filterUsers = allUsers.Where(x => x.TenantId.Equals(currentUserRequest.Result.TenantId)).Select(x => new SampleGetUserViewModel(x)).ToList();
            return new SuccessResultModel<IEnumerable<SampleGetUserViewModel>>(filterUsers);
        }

        /// <summary>
        /// Find roles by id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GearRole>> FindRolesByIdAsync(IEnumerable<Guid> ids)
        {
            var data = new List<GearRole>();
            foreach (var id in ids)
            {
                var role = await RoleManager.FindByIdAsync(id.ToString());
                if (role != null) data.Add(role);
            }

            return data;
        }

        /// <summary>
        /// Find roles by names
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<GearRole>>> FindRolesByNamesAsync(IEnumerable<string> roles)
        {
            var data = new List<GearRole>();
            foreach (var role in roles)
            {
                var gRole = await RoleManager.FindByNameAsync(role);
                if (gRole == null) continue;
                data.Add(gRole);
            }
            return new SuccessResultModel<IEnumerable<GearRole>>(data);
        }

        /// <summary>
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<GearUser>>> GetUsersInRolesAsync(IEnumerable<GearRole> roles, Guid? tenantId = null)
        {
            var data = new List<GearUser>();
            foreach (var role in roles)
            {
                var users = await UserManager.GetUsersInRoleAsync(role.Name);
                data.AddRange(tenantId == null ? users : users.Where(x => x.TenantId.Equals(tenantId)).ToList());
            }

            return new SuccessResultModel<IEnumerable<GearUser>>(data.DistinctBy(x => x.Id).ToList());
        }

        /// <summary>
        /// Change user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeUserRolesAsync(Guid? userId, IEnumerable<Guid> roles)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null) return new NotFoundResultModel();
            var currentRolesRequest = await FindRolesByNamesAsync(await UserManager.GetRolesAsync(user));
            if (!currentRolesRequest.IsSuccess) return currentRolesRequest.ToBase();
            var currentRoles = currentRolesRequest.Result.ToList();
            var rolesIds = currentRoles.Select(x => x.Id).ToList();
            var (newRoles, excludeRoles) = rolesIds.GetDifferences(roles);
            if (newRoles.Any())
            {
                var roleNames = (await FindRolesByIdAsync(newRoles)).Select(x => x.Name).ToList();
                await UserManager.AddToRolesAsync(user, roleNames);
            }

            if (excludeRoles.Any())
            {
                var roleNames = (await FindRolesByIdAsync(excludeRoles)).Select(x => x.Name).ToList();
                await UserManager.RemoveFromRolesAsync(user, roleNames);
            }

            return new SuccessResultModel<object>().ToBase();
        }

        /// <summary>
        /// Delete user permanently
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteUserPermanently(Guid? userId)
        {
            if (userId == null) return new InvalidParametersResultModel();
            var result = new ResultModel();
            var user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                result.Errors.Add(new ErrorModel("User not found"));
                return result;
            }

            if (IsCurrentUser(userId.Value))
            {
                result.Errors.Add(new ErrorModel("You can't delete current user"));
                return result;
            }

            if (!user.IsEditable)
            {
                result.Errors.Add(new ErrorModel("This user cannot be deleted"));
                return result;
            }

            var deleteResult = await UserManager.DeleteAsync(user);
            if (deleteResult.Succeeded)
            {
                IdentityEvents.Users.UserDelete(new UserDeleteEventArgs
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    UserId = user.Id
                });
                return new SuccessResultModel<Guid>().ToBase();
            }

            return deleteResult.ToResultModel<object>().ToBase();
        }

        /// <summary>
        /// Check if user is current user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool IsCurrentUser(Guid id)
            => _httpContextAccessor.HttpContext.User.IsAuthenticated()
               && id.Equals(_httpContextAccessor.HttpContext.User.Identity.Name.ToGuid());
    }
}