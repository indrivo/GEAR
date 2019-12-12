namespace GR.Calendar.Razor.ViewModels
{
    public class ExternalCalendarProviderSettingsViewModel
    {
        /// <summary>
        /// Provider name
        /// </summary>
        public string ProviderName { get; set; }


        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Is enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Icon
        /// </summary>
        public virtual string FontAwesomeIcon { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public ExternalCalendarUser User { get; set; } = new ExternalCalendarUser();
    }

    public class ExternalCalendarUser
    {
        /// <summary>
        /// Is authorized state
        /// </summary>
        public bool IsAuthorized { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        public string EmailAdress { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
