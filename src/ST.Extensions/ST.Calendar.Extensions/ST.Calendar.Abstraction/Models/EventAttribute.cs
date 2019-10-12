using System;

namespace ST.Calendar.Abstractions.Models
{
    public class EventAttribute
    {
        /// <summary>
        /// Event
        /// </summary>
        public virtual CalendarEvent Event { get; set; }
        public virtual Guid EventId { get; set; }

        /// <summary>
        /// Attribute name
        /// </summary>
        public virtual string AttributeName { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public virtual string Value { get; set; }
    }
}
