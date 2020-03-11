using System.Collections.Generic;
using GR.Core;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Notifications.Razor.Helpers
{
    public class NotificationsMenuInitializer: BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = NotificationsRazorResources.Menu.Notifications,
                    Name = "Notifications",
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Href = MenuResources.MenuItems.None,
                    Translate = "notifications",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig
                {
                    Id = NotificationsRazorResources.Menu.Subscriptions,
                    Name = "Notifications subscriptions",
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Href = "/NotificationSubscriptions",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                }
            }
        };
    }
}
