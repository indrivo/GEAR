using ST.Core;

namespace ST.Notifications.Abstractions.Models.Notifications
{
    public class UserEmailFolders : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsSystem { get; set; }
    }
}
