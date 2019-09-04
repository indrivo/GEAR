using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Identity.Abstractions.Models;
using ST.Identity.Abstractions.Models.AddressModels;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.Abstractions.Models.Permmisions;
using ST.Identity.Abstractions.Models.UserProfiles;

namespace ST.Identity.Abstractions
{
    public interface IIdentityContext : IDbContext
    {
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