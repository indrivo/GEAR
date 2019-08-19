using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ST.Core.Helpers;

namespace ST.Identity.Abstractions
{
    public interface IUserManager<TUser> where TUser : ApplicationUser
    {
        UserManager<TUser> UserManager { get; }
        RoleManager<ApplicationRole> RoleManager { get; }
        Task<ResultModel<ApplicationUser>> GetCurrentUserAsync();
    }
}