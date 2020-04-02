using GR.Identity.Abstractions;
using GR.Identity.Groups.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Groups.Abstractions
{
    public interface IGroupContext : IIdentityContext
    {
        DbSet<Group> Groups { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }
        DbSet<GroupPermission> GroupPermissions { get; set; }
    }
}
