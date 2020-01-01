using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Calendar.Abstractions.Enums;
using GR.Core;

namespace GR.Calendar.Abstractions.Models
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class CalendarEvent : BaseModel
    {
        /// <summary>
        /// Event name
        /// </summary>
        [Required]
        [MaxLength(50)]
        public virtual string Title { get; set; }

        /// <summary>
        /// Event description
        /// </summary>
        public virtual string Details { get; set; }

        /// <summary>
        /// Event location
        /// </summary>
        public virtual string Location { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [Required]
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [Required]
        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// Owner
        /// </summary>
        [Required]
        public virtual Guid Organizer { get; set; }

        /// <summary>
        /// Synced with external calendars
        /// </summary>
        public virtual bool Synced { get; set; }

        /// <summary>
        /// Event priority
        /// </summary>
        public virtual EventPriority Priority { get; set; } = EventPriority.Low;

        /// <summary>
        /// Event member
        /// </summary>
        public virtual ICollection<EventMember> EventMembers { get; set; } = new List<EventMember>();

        /// <summary>
        /// Attributes
        /// </summary>
        public virtual ICollection<EventAttribute> Attributes { get; set; } = new List<EventAttribute>();

        /// <summary>
        /// Minutes to remind
        /// </summary>
        public virtual int MinutesToRemind { get; set; } = 15;

        /// <summary>
        /// Remind sent
        /// </summary>
        public virtual bool RemindSent { get; set; }
    }
}
