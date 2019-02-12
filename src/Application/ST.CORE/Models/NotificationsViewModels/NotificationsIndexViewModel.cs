using System.Collections.Generic;

namespace ST.CORE.Models.NotificationsViewModels
{
	public class NotificationsIndexViewModel
	{
		public bool Sended { get; set; }
		public ICollection<NotificationMessageViewModel> Notifications { get; set; }
	}
}
