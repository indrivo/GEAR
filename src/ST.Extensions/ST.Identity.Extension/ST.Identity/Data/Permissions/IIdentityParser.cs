using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ST.Identity.Data.Permissions
{
    public interface IIdentityParser<TUser> where TUser: IdentityUser
    {
        Task<TUser> Parse(IPrincipal principal);
    }
}
