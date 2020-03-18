using System;
using GR.Core.Abstractions;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.Models.MultiTenants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Abstractions
{
    public interface IIdentityContext : IDbContext
    {
        DbSet<GearRole> Roles { get; set; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
        DbSet<GearUser> Users { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<StateOrProvince> StateOrProvinces { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<District> Districts { get; set; }
    }
}