using System;
using GR.Calendar.Abstractions.Enums;

namespace GR.Calendar.Abstractions.Models
{
    public class EventMember
    {
        /// <summary>
        /// Event reference
        /// </summary>
        public virtual CalendarEvent Event { get; set; }
        public virtual Guid EventId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Member acceptance
        /// </summary>
        public virtual EventAcceptance Acceptance { get; set; } = EventAcceptance.Tentative;
    }
}
