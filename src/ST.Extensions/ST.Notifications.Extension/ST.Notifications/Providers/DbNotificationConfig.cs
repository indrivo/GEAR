using System.Collections.Generic;
using ST.Entities.Data;
using ST.Entities.ViewModels.Table;

namespace ST.Notifications.Providers
{
    public class DbNotificationConfig
    {
        public EntitiesDbContext DbContext { get; set; }

        public Dictionary<string, TableConfigViewModel> Tables { get; set; }
    }
}