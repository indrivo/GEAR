using GR.Core;

namespace GR.Notifications.Abstractions.Models.Notifications
{
    public class NotificationTypes : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
