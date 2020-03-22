using GR.Identity.Profile.Abstractions.Models;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Localization.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile.Abstractions
{
    public interface IProfileContext : ICountryContext
    {
        DbSet<Models.Profile> Profiles { get; set; }
        DbSet<RoleProfile> RoleProfiles { get; set; }
        DbSet<Address> UserAddresses { get; set; }
    }
}