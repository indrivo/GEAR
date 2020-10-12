using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GR.Identity
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Base implementation of gear user manager")]
    public class IdentityUserManager : IUserManager<GearUser>
    {
        #region Helpers

        /// <summary>
        /// Default roles
        /// </summary>
        protected readonly Collection<string> DefaultRoles = new Collection<string> {
            GlobalResources.Roles.USER,
            GlobalResources.Roles.ANONYMOUS_USER
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

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService<IDistributedCache> _distributedCache;

        /// <summary>
        /// Inject logger
        /// </summary>
        protected readonly ILogger<IdentityUserManager> Logger;

        #endregion

        public IdentityUserManager(UserManager<GearUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<GearRole> roleManager, IIdentityContext identityContext, ICacheService cacheService, IMapper mapper, ICacheService<IDistributedCache> distributedCache, ILogger<IdentityUserManager> logger)
        {
            UserManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            RoleManager = roleManager;
            IdentityContext = identityContext;
            _cacheService = cacheService;
            _mapper = mapper;
            _distributedCache = distributedCache;
            Logger = logger;
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
            if (!user.Email.IsNullOrEmpty())
            {
                var emailUser = await UserManager.FindByEmailAsync(user.Email);
                if (emailUser != null)
                {
                    Logger.LogWarning($"Cannot create user, user with {user.Email.ToLowerInvariant()} email already exists");
                    var emailFailResponse = new ResultModel<Guid>();
                    emailFailResponse.AddError($"User with {user.Email.ToLowerInvariant()} email already exists");
                    return emailFailResponse;
                }
            }

            var createResponse = await UserManager.CreateAsync(user, password);
            var response = createResponse.ToResultModel<Guid>();
            response.Result = user.Id;

            if (response.IsSuccess)
            {
                await AddDefaultRoles(user);
                IdentityEvents.Users.UserCreated(new UserCreatedEventArgs
                {
                    Email = user.Email,
                    UserId = user.Id,
                    UserName = user.UserName
                });

                return response;
            }

            Logger.LogWarning("Fail to create user, payload: {User}, errors: {Errors}", user.SerializeAsJson(), response.Errors.SerializeAsJson());
            return response;
        }

        /// <summary>
        /// Find user id in claims
        /// </summary>
        /// <returns></returns>
        public virtual ResultModel<Guid> FindUserIdInClaims()
        {
            var result = new ResultModel<Guid>();
            if (_httpContextAccessor.HttpContext == null || !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                result.AddError("Unauthorized user");
                return result;
            }

            return FindUserIdInClaims(_httpContextAccessor.HttpContext.User);
        }

        /// <summary>
        /// Find user 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual ResultModel<Guid> FindUserIdInClaims(ClaimsPrincipal user)
        {
            if (user == null) throw new NullReferenceException();
            var result = new ResultModel<Guid>();
            if (!user.Identity.IsAuthenticated)
            {
                result.AddError("Unauthorized user");
                return result;
            }

            var nameIdentifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            var id = nameIdentifierClaim?.Value?.ToGuid() ?? UserManager.GetUserId(user)?.ToGuid();

            if (id == null) return result;

            result.Result = id.Value;
            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<GearUser>> FindUserByIdAsync(Guid? userId)
        {
            if (userId == null) return InvalidParametersResultModel<GearUser>.Instance;
            var user = await UserManager.FindByIdAsync(userId.ToString());
            return user == null
                ? NotFoundResultModel<GearUser>.Instance
                : new SuccessResultModel<GearUser>(user);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<GearUser>> GetCurrentUserAsync()
        {
            var idResult = FindUserIdInClaims();
            if (!idResult.IsSuccess) return idResult.Map<GearUser>();

            var result = new ResultModel<GearUser>();
            var user = await UserManager.FindByIdAsync(idResult.Result.ToString());
            result.IsSuccess = user != null;
            result.Result = user;
            return result;
        }

        /// <summary>
        /// Get current user info with fields of base identity
        /// </summary>
        /// <returns></returns>
        public virtual Task<ResultModel<IdentityUser<Guid>>> GetCurrentBaseUserAsync()
        {
            var idResult = FindUserIdInClaims();
            if (!idResult.IsSuccess) return Task.FromResult(idResult.Map<IdentityUser<Guid>>());
            var result = new ResultModel<IdentityUser<Guid>>();

            var user = UserManager.Users
                .AsNoTracking()
                .Where(x => x.Id == idResult.Result)
                .Select(x => new IdentityUser<Guid>
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    EmailConfirmed = x.EmailConfirmed,
                    ConcurrencyStamp = x.ConcurrencyStamp,
                    PhoneNumber = x.PhoneNumber,
                    PhoneNumberConfirmed = x.PhoneNumberConfirmed,
                    SecurityStamp = x.SecurityStamp
                }).FirstOrDefault();

            result.IsSuccess = user != null;
            result.Result = user;
            return Task.FromResult(result);
        }

        /// <summary>
        /// Get current user with custom fields
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public virtual Task<ResultModel<GearUser>> GetCurrentUserWithCustomFieldsAsync(Expression<Func<GearUser, GearUser>> configuration)
        {
            var idResult = FindUserIdInClaims();
            if (!idResult.IsSuccess) return Task.FromResult(idResult.Map<GearUser>());
            var result = new ResultModel<GearUser>();

            var user = UserManager.Users
                .Where(x => x.Id == idResult.Result)
                .Select(configuration).FirstOrDefault();

            result.IsSuccess = user != null;
            result.Result = user;
            return Task.FromResult(result);
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

        /// <summary>
        /// Get claims for current user
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Claim> GetCurrentUserClaims()
        {
            return _httpContextAccessor?.HttpContext?.User?.Claims?.ToList() ?? new List<Claim>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get request ip address
        /// </summary>
        /// <returns></returns>
        public virtual string GetRequestIpAddress() => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();

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

            if (user == null || roles == null) return new InvalidParametersResultModel();

            var exist = await UserManager.FindByNameAsync(user.UserName);
            if (exist == null)
            {
                result.AddError("User not found");
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
        /// Delete user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteUserAsync(Guid? userId)
            => await IdentityContext.DisableRecordAsync<GearUser>(userId);

        /// <summary>
        /// Restore user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RestoreUserAsync(Guid? userId)
            => await IdentityContext.ActivateRecordAsync<GearUser>(userId);

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
        public async Task<ResultModel<IEnumerable<UserInfoViewModel>>> GetUsersInRoleForCurrentCompanyAsync([Required] string roleName)
        {
            if (roleName.IsNullOrEmpty()) return new InvalidParametersResultModel<IEnumerable<UserInfoViewModel>>();
            var currentUserRequest = await GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return currentUserRequest.Map<IEnumerable<UserInfoViewModel>>();
            var allUsers = await UserManager.GetUsersInRoleAsync(roleName);
            var filterUsers = allUsers.Where(x => x.TenantId
                .Equals(currentUserRequest.Result.TenantId)).ToList();

            var mapped = _mapper.Map<IEnumerable<UserInfoViewModel>>(filterUsers);
            return new SuccessResultModel<IEnumerable<UserInfoViewModel>>(mapped);
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
                result.AddError("User not found");
                return result;
            }

            if (IsCurrentUser(userId.Value))
            {
                result.AddError("You can't delete current user");
                return result;
            }

            if (!user.IsEditable)
            {
                result.AddError("This user cannot be deleted");
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


        /// <summary>
        /// Get user image
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<byte[]>> GetUserImageAsync(Guid? userId)
        {
            if (userId == null) return new InvalidParametersResultModel<byte[]>();

            var requestImageResult = await _cacheService.GetOrSetResponseAsync<ResultModel<byte[]>>($"image_user_{userId}", async () =>
            {
                var user = (await FindUserByIdAsync(userId)).Result;

                if (user == null) return new NotFoundResultModel<byte[]>();
                if (user.UserPhoto != null && user.UserPhoto.Length > 0) return new SuccessResultModel<byte[]>(user.UserPhoto);

                var defaultImage = GetUserDefaultImage();
                return new SuccessResultModel<byte[]>(defaultImage);

            });

            return requestImageResult;
        }

        /// <summary>
        /// Remove user photo
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveUserPhotoAsync()
        {
            var resultModel = new ResultModel();
            var currentUser = (await GetCurrentUserAsync()).Result;
            if (currentUser == null) return new UserNotFoundResult<object>().ToBase();

            currentUser.UserPhoto = null;
            var result = await UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                await _cacheService.RemoveAsync($"image_user_{currentUser.Id}");
                return resultModel;
            }

            resultModel.IsSuccess = false;
            resultModel.AppendIdentityErrors(result.Errors);
            return resultModel;
        }

        /// <summary>
        /// Change user photo
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeUserPhotoAsync(IFormFile image)
        {
            var resultModel = new ResultModel();
            if (image == null || image.Length == 0)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Image is required" });
                return resultModel;
            }

            var currentUser = (await GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "User not found" });
                return resultModel;
            }

            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                currentUser.UserPhoto = memoryStream.ToArray();
            }

            var result = await UserManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                await _cacheService.RemoveAsync($"image_user_{currentUser.Id}");
                return resultModel;
            }

            resultModel.IsSuccess = false;
            resultModel.AppendIdentityErrors(result.Errors);
            return resultModel;
        }

        /// <summary>
        /// Get users with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<UserListItemViewModel>> GetAllUsersWithPaginationAsync(DTParameters parameters)
        {
            var request = await IdentityContext.Users.GetPagedAsDtResultAsync(parameters);
            return _mapper.Map<DTResult<UserListItemViewModel>>(request);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeUserPasswordAsync(string current, string next)
        {
            var resultModel = new ResultModel();
            var currentUser = (await GetCurrentUserAsync()).Result;
            if (currentUser == null) return new UserNotFoundResult<object>().ToBase();

            var result = await UserManager.ChangePasswordAsync(currentUser, current, next);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                IdentityEvents.Users.UserPasswordChange(new UserChangePasswordEventArgs
                {
                    Email = currentUser.Email,
                    UserName = currentUser.UserName,
                    UserId = currentUser.Id,
                    Password = next
                });
                return resultModel;
            }

            resultModel.AppendIdentityErrors(result.Errors);
            return resultModel;
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="current"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeUserPasswordAsync(GearUser user, string current, string next)
        {
            var resultModel = new ResultModel();
            var result = await UserManager.ChangePasswordAsync(user, current, next);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                IdentityEvents.Users.UserPasswordChange(new UserChangePasswordEventArgs
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Password = next
                });
                return resultModel;
            }

            resultModel.AppendIdentityErrors(result.Errors);
            return resultModel;
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ChangeUserPasswordAsync(GearUser user, string password)
        {
            var resultModel = new ResultModel();
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            var result = await UserManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                IdentityEvents.Users.UserPasswordChange(new UserChangePasswordEventArgs
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Password = password
                });
                return resultModel;
            }

            resultModel.AppendIdentityErrors(result.Errors);
            return resultModel;
        }

        /// <summary>
        /// Find user by phone number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual async Task<GearUser> FindByPhoneNumberAsync(string phone)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phone));
            return user;
        }

        /// <summary>
        /// Update security time stamp and store in distributed cache
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> UpdateSecurityStampAsync(GearUser user)
        {
            var result = await UserManager.UpdateSecurityStampAsync(user);
            if (!result.Succeeded) return result.ToResultModel<string>();

            var securityStamp = await UserManager.GetSecurityStampAsync(user);
            await _distributedCache.SetAsync(user.Id + "-security-time-stamp", securityStamp);
            return new SuccessResultModel<string>();

        }

        /// <summary>
        /// Get security token from distributed cache and renew
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> GetSecurityStampAsync(Guid userId)
        {
            var cachedSecurityStamp = await _distributedCache.GetAsync<string>(userId + "-security-time-stamp");
            if (!cachedSecurityStamp.IsNullOrEmpty()) return new SuccessResultModel<string>(cachedSecurityStamp);

            var userResponse = await FindUserByIdAsync(userId);
            if (!userResponse.IsSuccess) return userResponse.Map<string>();
            var securityStamp = await UserManager.GetSecurityStampAsync(userResponse.Result);
            await _distributedCache.SetAsync(userId + "-security-time-stamp", securityStamp);
            return new SuccessResultModel<string>(securityStamp);
        }

        /// <summary>
        /// Return true if current logged user is admin
        /// </summary>
        /// <returns></returns>
        public virtual Task<bool> IsCurrentUserAdmin()
        {
            var claims = GetRolesFromClaims().ToList();
            return Task.FromResult(claims.Contains(GlobalResources.Roles.ADMINISTRATOR));
        }

        /// <summary>
        /// Set token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="tokenName"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetTokenAsync(Guid userId, string loginProvider, string tokenName, string tokenValue)
        {
            var userResult = await FindUserByIdAsync(userId);
            if (!userResult.IsSuccess) return userResult.ToBase();
            return await SetTokenAsync(userResult.Result, loginProvider, tokenName, tokenValue);
        }

        /// <summary>
        /// Set token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="loginProvider"></param>
        /// <param name="tokenName"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetTokenAsync(GearUser user, string loginProvider, string tokenName, string tokenValue)
        {
            Arg.NotNull(user, "User cannot be null");
            var setTokenResult = await UserManager
                .SetAuthenticationTokenAsync(user, loginProvider, tokenName, tokenValue);

            if (!setTokenResult.Succeeded)
            {
                return setTokenResult.ToResultModel();
            }
            return new SuccessResultModel();
        }

        #region Helpers

        /// <summary>
        /// Get default user image
        /// </summary>
        /// <returns></returns>
        private static byte[] GetUserDefaultImage()
        {
            var path = Path.Combine(AppContext.BaseDirectory, GlobalResources.Paths.EmbeddedResourcesPath + "/user.jpg");
            if (!File.Exists(path))
                return default;

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var binary = new BinaryReader(stream))
                {
                    var data = binary.ReadBytes((int)stream.Length);
                    return data;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return default;
        }

        #endregion
    }
}