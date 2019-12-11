using System;
using System.Collections.Generic;

namespace GR.Notifications.Razor.ViewModels.Subscriptions
{
    public class NotificationSubscribeViewModel
    {
        /// <summary>
        /// Notification subject
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Event name
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Template value
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public IEnumerable<Guid> Roles { get; set; }
    }
}
