using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Entities.Security.Abstractions.Models;

namespace GR.Entities.Security.Abstractions
{
    public interface IEntitySecurityDbContext : IDbContext
    {
        DbSet<EntityPermission> EntityPermissions { get; set; }
        DbSet<EntityFieldPermission> EntityFieldPermissions { get; set; }
        DbSet<EntityPermissionAccess> EntityPermissionAccesses { get; set; }
    }
}