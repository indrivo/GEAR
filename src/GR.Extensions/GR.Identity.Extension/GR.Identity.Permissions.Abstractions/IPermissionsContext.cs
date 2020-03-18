using GR.Core.Abstractions;
using GR.Identity.Abstractions.Models;
using GR.Identity.Permissions.Abstractions.Permissions;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Permissions.Abstractions
{
    public interface IPermissionsContext : IDbContext
    {
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<Permission> Permissions { get; set; }
    }
}