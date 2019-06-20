using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ST.Identity.LdapAuth.Abstractions.Models;

namespace ST.Identity.LdapAuth
{
    public class LdapSignInManager<TContext, TUser> : SignInManager<TUser> where TContext : DbContext where TUser : LdapUser
    {
        public LdapSignInManager(
            LdapUserManager<TUser> ldapUserManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<LdapSignInManager<TContext, TUser>> logger,
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
