namespace GR.Calendar.Abstractions.Events.EventArgs
{
    public class EventDeleteOrRestoredEventArgs : CalendarEventCreatedEventArgs
    {
        /// <summary>
        /// Event state
        /// </summary>
        public virtual bool InLife { get; set; }
    }
}