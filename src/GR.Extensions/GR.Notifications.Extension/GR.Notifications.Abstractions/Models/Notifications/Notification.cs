using System;

namespace GR.Notifications.Abstractions.Models.Notifications
{
    public class Notification
    {
        /// <summary>
        /// Notification id
        /// </summary>
        public Guid? Id { get; set; }

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
        public Guid NotificationTypeId { get; set; } = NotificationType.Info;

        /// <summary>
        /// User
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Send local
        /// </summary>
        public bool SendLocal { get; set; } = true;

        /// <summary>
        /// Send email
        /// </summary>
        public bool SendEmail { get; set; } = true;
    }
}
