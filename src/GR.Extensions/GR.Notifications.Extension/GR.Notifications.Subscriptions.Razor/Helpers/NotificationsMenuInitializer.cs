using System.Collections.Generic;
using GR.Core;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Notifications.Subscriptions.Razor.Helpers
{
    public class NotificationsMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig(NotificationSubscriptionsRazorResources.Menu.Notifications)
                {
                    Name = "Notifications",
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Href = MenuResources.MenuItems.None,
                    Translate = "notifications",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(NotificationSubscriptionsRazorResources.Menu.Subscriptions)
                {
                    Name = "Notifications subscriptions",
                    ParentMenuItemId = NotificationSubscriptionsRazorResources.Menu.Notifications,
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
