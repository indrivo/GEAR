using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GR.Identity.LdapAuth.Abstractions.Models;

namespace GR.Identity.LdapAuth.Abstractions
{
    public abstract class BaseLdapUserManager<TLdapUser> : UserManager<TLdapUser> where TLdapUser : LdapUser
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
        /// <param name="store"></param>
        /// <param name="optionsAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="userValidators"></param>
        /// <param name="passwordValidators"></param>
        /// <param name="keyNormalizer"></param>
        /// <param name="errors"></param>
        /// <param name="services"></param>
        /// <param name="logger"></param>
        protected BaseLdapUserManager(
            ILdapService<TLdapUser> ldapService,
            IUserStore<TLdapUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TLdapUser> passwordHasher,
            IEnumerable<IUserValidator<TLdapUser>> userValidators,
            IEnumerable<IPasswordValidator<TLdapUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<BaseLdapUserManager<TLdapUser>> logger)
            : base(
                store,
                optionsAccessor,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger)
        {
            LdapService = ldapService;
        }

        public abstract LdapUser GetAdministrator();
        public abstract Task<IdentityResult> DeleteUserAsync(string distinguishedName);
    }
}
