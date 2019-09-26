using System;

namespace ST.Calendar.Abstractions.Models
{
    public class CalendarEvent
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
    }
}
