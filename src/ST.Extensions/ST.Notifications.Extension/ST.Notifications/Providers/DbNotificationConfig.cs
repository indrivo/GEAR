using System.Collections.Generic;
using ST.Entities.Abstractions.ViewModels.Table;
using ST.Entities.Data;

namespace ST.Notifications.Providers
{
    public class DbNotificationConfig
    {
        public EntitiesDbContext DbContext { get; set; }

        public Dictionary<string, TableConfigViewModel> Tables { get; set; }
    }
}