using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.LdapAuth.Abstractions
{
    public abstract class BaseLdapUserManager<TLdapUser> where TLdapUser : LdapUser
    {
        /// <summary>
        /// Inject Ldap Service
        /// </summary>
        protected readonly ILdapService<TLdapUser> LdapService;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ldapService"></param>
        protected BaseLdapUserManager(
            ILdapService<TLdapUser> ldapService)
        {
            LdapService = ldapService;
        }

        public abstract IQueryable<TLdapUser> Users { get; }

        /// <summary>
        /// Get administrator
        /// </summary>
        /// <returns></returns>
        public abstract LdapUser GetAdministrator();

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <returns></returns>
        public abstract Task<IdentityResult> DeleteUserAsync(string distinguishedName);

        /// <summary>
        /// Checks the given password again the configured LDAP server.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract Task<bool> CheckPasswordAsync(TLdapUser user, string password);

        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public abstract Task<TLdapUser> FindByIdAsync(string userId);

        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public abstract Task<TLdapUser> FindByNameAsync(string userName);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract Task<IdentityResult> CreateAsync(TLdapUser user, string password);
    }
}