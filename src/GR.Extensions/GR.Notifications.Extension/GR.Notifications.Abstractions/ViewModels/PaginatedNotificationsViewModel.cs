using System.Collections.Generic;
using GR.Notifications.Abstractions.Models.Notifications;

namespace GR.Notifications.Abstractions.ViewModels
{
    public class PaginatedNotificationsViewModel
    {
        public uint Page { get; set; } = 1;
        public uint Total { get; set; } = 0;
        public uint PerPage { get; set; } = 10;
        public IEnumerable<Notification> Notifications { get; set; }
    }
}
