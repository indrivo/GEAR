using System.Collections.Generic;

namespace GR.Identity.Models.NotificationsViewModels
{
    public class NotificationsIndexViewModel
    {
        public bool Sended { get; set; }
        public ICollection<NotificationMessageViewModel> Notifications { get; set; }
    }
}