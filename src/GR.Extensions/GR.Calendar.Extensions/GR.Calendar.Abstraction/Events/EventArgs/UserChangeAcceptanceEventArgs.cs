using System;
using GR.Calendar.Abstractions.Models;

namespace GR.Calendar.Abstractions.Events.EventArgs
{
    public sealed class UserChangeAcceptanceEventArgs : System.EventArgs
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Event id
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Organizer
        /// </summary>
        public Guid Organizer { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public EventMember Member { get; set; }

        /// <summary>
        /// Acceptance
        /// </summary>
        public string AcceptanceState { get; set; }
    }
}
