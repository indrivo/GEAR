using System.Collections.Generic;

namespace ST.Identity.Models.NotificationsViewModels
{
	public class NotificationCreateViewModel
	{
		public string Subject { get; set; }
		public string Message { get; set; }
		public ICollection<string> Recipients { get; set; }
	}
}
