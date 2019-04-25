using System.Collections.Generic;

namespace ST.Email.Razor.ViewModels.NotificationsViewModels
{
	public class NotificationsIndexViewModel
	{
		public bool Sended { get; set; }
		public ICollection<NotificationMessageViewModel> Notifications { get; set; }
	}
}
