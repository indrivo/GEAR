using Microsoft.EntityFrameworkCore;
using GR.Calendar.Abstractions.Models;
using GR.Core.Abstractions;

namespace GR.Calendar.Abstractions
{
    public interface ICalendarDbContext : IDbContext
    {
        /// <summary>
        /// Events
        /// </summary>
        DbSet<CalendarEvent> CalendarEvents { get; set; }

        /// <summary>
        /// Event members
        /// </summary>
        DbSet<EventMember> EventMembers { get; set; }

        /// <summary>
        /// Preferences
        /// </summary>
        DbSet<UserProviderSyncPreference> UserProviderSyncPreferences { get; set; }

        /// <summary>
        /// Provider tokens
        /// </summary>
        DbSet<ExternalProviderToken> ExternalProviderTokens { get; set; }

        /// <summary>
        /// Event attributes
        /// </summary>
        DbSet<EventAttribute> Attributes { get; set; }
    }
}