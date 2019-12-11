namespace GR.Calendar.Abstractions.ExternalProviders.Helpers
{
    public class ExternalCalendarUser
    {
        /// <summary>
        /// Display name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Email adress
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        public  virtual  string ImageUrl { get; set; }
    }
}
