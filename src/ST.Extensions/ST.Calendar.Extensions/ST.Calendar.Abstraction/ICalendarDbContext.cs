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
    }
}