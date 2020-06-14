using GR.Identity.Abstractions;
using GR.UserPreferences.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.UserPreferences.Abstractions
{
    public interface IUserPreferencesContext : IIdentityContext
    {
        /// <summary>
        /// User preferences
        /// </summary>
        DbSet<UserPreference> UserPreferences { get; set; }
    }
}