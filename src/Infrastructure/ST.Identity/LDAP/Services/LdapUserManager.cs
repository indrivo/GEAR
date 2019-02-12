using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Identity.LDAP.Models;

namespace ST.Identity.LDAP.Services
{
    public class LdapUserManager : UserManager<ApplicationUser>
    {
        /// <summary>
        /// Inject Ldap Service
        /// </summary>
        private readonly ILdapService _ldapService;
        /// <summary>
        /// Inject context
        /// </summary>

        private readonly ApplicationDbContext _context;
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
        public LdapUserManager(
            ILdapService ldapService,
            IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<LdapUserManager> logger, ApplicationDbContext context)
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
            _ldapService = ldapService;
            _context = context;
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
        public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return Task.Run(() => _ldapService.Authenticate(user.DistinguishedName, password));
        }
        /// <inheritdoc />
        /// <summary>
        /// Find user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }
        /// <inheritdoc />
        /// <summary>
        /// Find user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override Task<ApplicationUser> FindByNameAsync(string userName)
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
        public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
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
        /// <summary>
        /// Change Identity password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task ChangeIdentityPassword(ApplicationUser user, string password)
        {
            try
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                var hashedPassword = hasher.HashPassword(user, password);
                user.PasswordHash = hashedPassword;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <inheritdoc />
        /// <summary>
        /// Get Ldap users
        /// </summary>
        public override IQueryable<ApplicationUser> Users => _ldapService.GetAllUsers().AsQueryable();
        /// <summary>
        /// Get local users
        /// </summary>
        public IQueryable<ApplicationUser> LocalUsers =>
            _context.Users.Where(x => x.AuthenticationType.Equals(AuthenticationType.Local));
    }
}
