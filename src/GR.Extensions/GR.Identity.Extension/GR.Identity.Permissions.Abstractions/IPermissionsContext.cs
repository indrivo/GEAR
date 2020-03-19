using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions.Permissions;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Permissions.Abstractions
{
    public interface IPermissionsContext : IIdentityContext
    {
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<Permission> Permissions { get; set; }
    }
}