using GR.Core.Abstractions;
using GR.Localization.Abstractions.Models.Countries;
using Microsoft.EntityFrameworkCore;

namespace GR.Localization.Abstractions
{
    public interface ICountryContext : IDbContext
    {
        DbSet<Country> Countries { get; set; }
        DbSet<StateOrProvince> StateOrProvinces { get; set; }
    }
}