using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.LDAP.Services
{
    public class LdapSignInManager<TContext> : SignInManager<ApplicationUser> where TContext : ApplicationDbContext
    {
        public LdapSignInManager(
            LdapUserManager<TContext> ldapUserManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<LdapSignInManager<TContext>> logger,
            IAuthenticationSchemeProvider schemes)
            : base(
                ldapUserManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger,
                schemes)
        {
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockOutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);

            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, rememberMe, lockOutOnFailure);
        }
    }
}
