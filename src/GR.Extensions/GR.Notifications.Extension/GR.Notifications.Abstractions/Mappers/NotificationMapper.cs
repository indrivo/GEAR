using GR.Notifications.Abstractions.Models.Notifications;

namespace GR.Notifications.Abstractions.Mappers
{
    public static class NotificationMapper
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public static SystemNotifications Map(Notification notification)
        {
            if (notification == null) return new SystemNotifications();

            return new SystemNotifications
            {
                Id = notification.Id.GetValueOrDefault(),
                Content = notification.Content,
                NotificationTypeId = notification.NotificationTypeId,
                Subject = notification.Subject,
                UserId = notification.UserId
            };
        }
    }
}
