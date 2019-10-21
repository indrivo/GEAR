using System;
using GR.Core;

namespace GR.Notifications.Abstractions.Models.Notifications
{
    public sealed class SystemNotifications : BaseModel
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public Guid NotificationTypeId { get; set; }
        public Guid UserId { get; set; }
    }

    public static class NotificationType
    {
        public static Guid Add = Guid.Parse("8C263760-B391-4BCC-961A-C7E049CA468F");
        public static Guid Delete = Guid.Parse("72F151F2-68AB-4473-8069-519D39B31612");
        public static Guid Edit = Guid.Parse("36B41600-E458-476A-A4BD-5CE788B087F5");
        public static Guid Info = Guid.Parse("3FB5D5FB-2B23-4FBB-A094-2E21557C9401");
    }
}
