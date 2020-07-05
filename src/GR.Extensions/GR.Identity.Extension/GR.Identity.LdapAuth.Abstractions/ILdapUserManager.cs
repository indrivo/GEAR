using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Abstractions.ViewModels.UserViewModels;

namespace GR.Identity.LdapAuth.Abstractions
{
    public interface ILdapUserManager<TLdapUser> where TLdapUser : LdapUser
    {
        IQueryable<TLdapUser> Users { get; }

        /// <summary>
        /// Get administrator
        /// </summary>
        /// <returns></returns>
        LdapUser GetAdministrator();

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <returns></returns>
        Task<IdentityResult> DeleteUserAsync(string distinguishedName);

        /// <summary>
        /// Checks the given password again the configured LDAP server.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ResultModel> CheckPasswordAsync(TLdapUser user, string password);

        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<TLdapUser> FindByIdAsync(string userId);

        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<TLdapUser> FindByNameAsync(string userName);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> CreateAsync(TLdapUser user, string password);

        /// <summary>
        /// Get not added ldap users
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<LdapUser>>> GetNotAddedLdapUsersAsync();

        /// <summary>
        /// Import ad use
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> ImportAdUserAsync([Required] string userName);

        /// <summary>
        /// Import ad user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> ImportAdUserAsync([Required] string userName, string password);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<UserListItemViewModel>> GetAllLdapUsersWithPaginationAsync(DTParameters parameters);
    }
}