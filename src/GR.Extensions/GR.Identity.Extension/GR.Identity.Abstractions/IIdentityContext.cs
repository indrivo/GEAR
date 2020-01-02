using GR.Core.Abstractions;
using GR.Identity.Abstractions.Models;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Abstractions.Models.Permmisions;
using GR.Identity.Abstractions.Models.UserProfiles;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Abstractions
{
    public interface IIdentityContext : IDbContext
    {
        DbSet<GearRole> Roles { get; set; }
        DbSet<GearUser> Users { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        DbSet<AuthGroup> AuthGroups { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<GroupPermission> GroupPermissions { get; set; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<Profile> Profiles { get; set; }
        DbSet<RoleProfile> RoleProfiles { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<StateOrProvince> StateOrProvinces { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<District> Districts { get; set; }
    }
}