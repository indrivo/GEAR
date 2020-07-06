using GR.Identity.LdapAuth.Abstractions;
using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Identity.LdapAuth.Abstractions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.LdapAuth
{
    public class LdapUserManager<TLdapUser> : ILdapUserManager<TLdapUser> where TLdapUser : LdapUser
    {
        #region Injectable

        /// <summary>
        /// Inject ldap service
        /// </summary>
        private readonly ILdapService<TLdapUser> _ldapService;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IIdentityContext _identityContext;

        #endregion


        public LdapUserManager(ILdapService<TLdapUser> ldapService, IMapper mapper, IUserManager<GearUser> userManager, IIdentityContext identityContext)
        {
            _ldapService = ldapService;
            _mapper = mapper;
            _userManager = userManager;
            _identityContext = identityContext;
        }

        /// <summary>
        /// Get Administrator
        /// </summary>
        /// <returns></returns>
        public LdapUser GetAdministrator()
        {
            return _ldapService.GetAdministrator();
        }

        /// <inheritdoc />
        /// <summary>
        /// Checks the given password again the configured LDAP server.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<ResultModel> CheckPasswordAsync(TLdapUser user, string password)
            => Task.FromResult(_ldapService.Authenticate(user.DistinguishedName, password));

        /// <inheritdoc />
        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<TLdapUser> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }

        /// <inheritdoc />
        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<TLdapUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(_ldapService.GetUserByUserName(userName));
        }

        /// <inheritdoc />
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateAsync(TLdapUser user, string password)
        {
            try
            {
                _ldapService.AddUser(user, password);
            }
            catch (Exception e)
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "LdapUserCreateFailed", Description = e.Message ?? "The user could not be created." }));
            }

            return await Task.FromResult(IdentityResult.Success);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteUserAsync(string distinguishedName)
        {
            try
            {
                _ldapService.DeleteUser(distinguishedName);
            }
            catch (Exception e)
            {
                return await Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "LdapUserDeleteFailed", Description = e.Message ?? "The user could not be deleted." }));
            }

            return await Task.FromResult(IdentityResult.Success);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get Ldap users
        /// </summary>
        public IQueryable<TLdapUser> Users => _ldapService.GetAllUsers().AsQueryable();

        /// <summary>
        /// Get not added ldap users
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<LdapUser>>> GetNotAddedLdapUsersAsync()
        {
            var result = new ResultModel<IEnumerable<LdapUser>>();
            var addedUsers = await _identityContext.Users
                .Where(x => x.AuthenticationType
                    .Equals(AdResources.AddAuthenticationType)).ToListAsync();

            var mapped = _mapper.Map<IEnumerable<LdapUser>>(addedUsers).ToList();
            var users = Users.ToList();
            if (addedUsers.Any())
            {
                users = users.Where(x => !mapped.Any(c => c.UserName.Equals(x.SamAccountName))).ToList();
            }

            result.IsSuccess = true;
            result.Result = users;
            return result;
        }

        /// <summary>
        /// import ldap user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> ImportAdUserAsync([Required] string userName)
            => await ImportAdUserAsync(userName, AdResources.DefaultPassword);

        /// <summary>
        /// import ad user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> ImportAdUserAsync([Required] string userName, string password)
        {
            var result = new ResultModel<Guid>();
            if (string.IsNullOrEmpty(userName))
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"Invalid username : {userName}"));
                return result;
            }

            var exists = await _userManager.UserManager.FindByNameAsync(userName);
            if (exists != null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"UserName {userName} exists!"));
                return result;
            }

            var ldapUser = await FindByNameAsync(userName);
            if (ldapUser == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"There is no AD user with this username : {userName}"));
                return result;
            }

            var user = _mapper.Map<GearUser>(ldapUser);
            user.Id = Guid.NewGuid();
            user.UserName = ldapUser.SamAccountName;
            user.Email = ldapUser.EmailAddress;
            user.AuthenticationType = AdResources.AddAuthenticationType;
            result.IsSuccess = true;
            _userManager.UserManager.Options.Password.RequireNonAlphanumeric = false;
            var req = await _userManager.UserManager.CreateAsync(user, password);
            if (!req.Succeeded)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"Fail to add user : {userName}"));
                result.IsSuccess = false;
            }
            else
            {
                IdentityEvents.Users.UserCreated(new UserCreatedEventArgs
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    UserId = user.Id
                });
                result.Result = user.Id;
            }

            return result;
        }

        /// <summary>
        /// Get users with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<UserListItemViewModel>> GetAllLdapUsersWithPaginationAsync(DTParameters parameters)
        {
            var request = await _identityContext.Users
                .Where(x => x.AuthenticationType == AdResources.AddAuthenticationType)
                .GetPagedAsDtResultAsync(parameters);
            return _mapper.Map<DTResult<UserListItemViewModel>>(request);
        }
    }
}