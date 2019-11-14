using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Notifications.Abstractions.Models.Data
{
    public class NotificationSubscription
    {
        /// <summary>
        /// Role id
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Notification event id
        /// </summary>
        [Required]
        public string NotificationEventId { get; set; }
        /// <summary>
        /// Notification event reference
        /// </summary>
        public  NotificationEvent NotificationEvent { get; set; }
    }
}
