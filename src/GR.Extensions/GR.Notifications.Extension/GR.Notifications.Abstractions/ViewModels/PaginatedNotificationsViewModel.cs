using System.Collections.Generic;

namespace GR.Notifications.Abstractions.ViewModels
{
    public class PaginatedNotificationsViewModel
    {
        public uint Page { get; set; } = 1;
        public uint Total { get; set; } = 0;
        public uint PerPage { get; set; } = 10;
        public List<Dictionary<string, object>> Notifications { get; set; }
    }
}
