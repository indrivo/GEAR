using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions.Models;
using ST.Core.Abstractions;

namespace ST.Calendar.Abstractions
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