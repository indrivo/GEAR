using System.Security.Claims;
using GR.Identity.Abstractions.Models.GroupModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Abstractions
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