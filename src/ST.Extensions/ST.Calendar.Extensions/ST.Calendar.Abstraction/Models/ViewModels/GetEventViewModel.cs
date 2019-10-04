namespace ST.Calendar.Abstractions.Models.ViewModels
{
    public class GetEventViewModel : BaseEventViewModel
    {
        public override bool Synced { get; set; }

        /// <summary>
        /// Event duration
        /// </summary>
        public virtual EventDuration Duration
        {
            get
            {
                var time = EndDate - StartDate;
                return new EventDuration(time.Days, time.Hours, time.Minutes, time.Seconds);
            }
        }
    }
}
