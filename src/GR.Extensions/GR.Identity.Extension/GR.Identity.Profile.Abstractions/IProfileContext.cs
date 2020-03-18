using GR.Core.Abstractions;
using GR.Identity.Profile.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile.Abstractions
{
    public interface IProfileContext : IDbContext
    {
        DbSet<Models.Profile> Profiles { get; set; }

        DbSet<RoleProfile> RoleProfiles { get; set; }
    }
}