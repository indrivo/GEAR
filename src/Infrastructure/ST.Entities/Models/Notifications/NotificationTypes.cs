using ST.BaseRepository;

namespace ST.Entities.Models.Notifications
{
    public class NotificationTypes : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
