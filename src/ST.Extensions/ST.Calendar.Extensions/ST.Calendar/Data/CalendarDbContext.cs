using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Calendar.Abstractions;
using ST.Calendar.Abstractions.Models;
using ST.Core.Abstractions;

namespace ST.Calendar.Data
{
    public class CalendarDbContext : TrackerDbContext, ICalendarDbContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this, is used on audit 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string Schema = "Calendar";

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Events
        /// </summary>
        public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Events
        /// </summary>
        public virtual DbSet<EventMember> EventMembers { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            builder.Entity<CalendarEvent>()
                .HasMany(x => x.EventMembers)
                .WithOne(x => x.Event)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventMember>().HasKey(x => new { x.EventId, x.UserId });
            builder.Entity<CalendarEvent>().HasIndex(x => new { x.StartDate, x.EndDate });
            builder.Entity<CalendarEvent>().HasIndex(x => new { Owner = x.Organizer });
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual DbSet<T> SetEntity<T>() where T : class, IBaseModel => Set<T>();
    }
}