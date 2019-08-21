using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Entities.Security.Abstractions.Models;

namespace ST.Entities.Security.Abstractions
{
    public interface IEntitySecurityDbContext : IDbContext
    {
        DbSet<EntityPermission> EntityPermissions { get; set; }
        DbSet<EntityFieldPermission> EntityFieldPermissions { get; set; }
        DbSet<EntityPermissionAccess> EntityPermissionAccesses { get; set; }
    }
}