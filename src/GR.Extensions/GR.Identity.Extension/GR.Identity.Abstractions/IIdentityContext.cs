using System;
using GR.Core.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Abstractions
{
    public interface IIdentityContext : IDbContext
    {
        DbSet<GearRole> Roles { get; set; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
        DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }
        DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }
        DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }
        DbSet<GearUser> Users { get; set; }
        DbSet<Tenant> Tenants { get; set; }
    }
}