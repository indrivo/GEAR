using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Backup.Razor.Helpers
{
    public class BackupMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig(BackupResources.Menu.BackupsSection)
                {
                    Name = "Backups",
                    ParentMenuItemId = MenuResources.MenuItems.DatabaseItem,
                    Href = "/DataBaseBackup"
                }
            }
        };
    }
}
