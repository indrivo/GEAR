using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Threading.Tasks;

namespace GR.Identity.Data.Permissions
{
    public interface IIdentityParser<TUser> where TUser : IdentityUser<Guid>
    {
        Task<TUser> Parse(IPrincipal principal);
    }
}