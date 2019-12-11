namespace GR.Calendar.Abstractions.Models
{
    public sealed class EventDuration
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EventDuration() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public EventDuration(int days, int hours, int minutes, int seconds)
        {
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        /// <summary>
        /// Days
        /// </summary>
        public int Days { get; set; } = 0;

        /// <summary>
        /// Hours
        /// </summary>
        public int Hours { get; set; } = 0;

        /// <summary>
        /// Minutes
        /// </summary>
        public int Minutes { get; set; } = 0;

        /// <summary>
        /// Seconds
        /// </summary>
        public int Seconds { get; set; } = 0;
    }
}
