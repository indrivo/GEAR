using System;

namespace GR.Calendar.Abstractions.Events.EventArgs
{
    public class EventUpdatedEventArgs : CalendarEventCreatedEventArgs
    {
        public virtual bool Synced { get; set; }
    }
}
