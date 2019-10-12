using System;

namespace ST.Calendar.Abstractions.Events.EventArgs
{
    public class EventDeleteOrRestoredEventArgs : System.EventArgs
    {
        /// <summary>
        /// Event id
        /// </summary>
        public virtual Guid? EventId { get; set; }

        /// <summary>
        /// Event title
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Event state
        /// </summary>
        public virtual bool InLife { get; set; }
    }
}
