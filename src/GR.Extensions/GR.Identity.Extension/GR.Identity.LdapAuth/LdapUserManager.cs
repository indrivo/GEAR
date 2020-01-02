using GR.Identity.LdapAuth.Abstractions;
using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.LdapAuth
{
    public class LdapUserManager<TLdapUser> : BaseLdapUserManager<TLdapUser> where TLdapUser : LdapUser
    {
        public LdapUserManager(ILdapService<TLdapUser> ldapService) : base(ldapService)
        {
        }

        /// <summary>
        /// Get Administrator
        /// </summary>
        /// <returns></returns>
        public override LdapUser GetAdministrator()
        {
            return LdapService.GetAdministrator();
        }

        /// <inheritdoc />
        /// <summary>
        /// Checks the given password again the configured LDAP server.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override Task<bool> CheckPasswordAsync(TLdapUser user, string password)
        {
            return Task.Run(() => LdapService.Authenticate(user.DistinguishedName, password));
        }

        /// <inheritdoc />
        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override Task<TLdapUser> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }

        /// <inheritdoc />
        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override Task<TLdapUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(LdapService.GetUserByUserName(userName));
        }

        /// <inheritdoc />
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override async Task<IdentityResult> CreateAsync(TLdapUser user, string password)
        {
            try
            {
                LdapService.AddUser(user, password);
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
        public override async Task<IdentityResult> DeleteUserAsync(string distinguishedName)
        {
            try
            {
                LdapService.DeleteUser(distinguishedName);
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
        public override IQueryable<TLdapUser> Users => LdapService.GetAllUsers().AsQueryable();
    }
}