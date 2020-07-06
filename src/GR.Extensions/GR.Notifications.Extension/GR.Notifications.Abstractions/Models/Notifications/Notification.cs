using System;
using GR.Core;

namespace GR.Notifications.Abstractions.Models.Notifications
{
    public class Notification : BaseModel
    {
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Content body
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Notification type
        /// </summary>
        public Guid NotificationTypeId { get; set; } = Notifications.NotificationType.Info;
        public NotificationTypes NotificationType { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public Guid UserId { get; set; }
    }
}