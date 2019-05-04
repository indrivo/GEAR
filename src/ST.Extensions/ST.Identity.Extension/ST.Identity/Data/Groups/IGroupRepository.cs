using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ST.Identity.Data.Groups
{
    public interface IGroupRepository<T, in TUser> where T : DbContext where TUser : IdentityUser
    {
        GroupResult AddUserToGroup(TUser user, string groupName);
        GroupResult AddUserToGroup(ClaimsPrincipal user, string groupName);
        GroupResult RemoveUserFromGroup(ClaimsPrincipal user, string groupName);
        GroupResult RemoveUserFromGroup(TUser user, string groupName);
        bool UserIsInGroup(TUser user, string groupName);
        bool UserIsInGroup(ClaimsPrincipal user, string groupName);
    }
}
