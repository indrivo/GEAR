using System.Collections.Generic;
using GR.Core;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Dashboard.Razor.Helpers
{
    public class DashboardMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder =>
        new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig(DashboardRazorResources.Menu.DashboardConfiguration)
                {
                    Name = "Dashboard configuration",
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Href = MenuResources.MenuItems.None,
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(DashboardRazorResources.Menu.AllDashboard)
                {
                    Name = "All dashboards",
                    ParentMenuItemId = DashboardRazorResources.Menu.DashboardConfiguration,
                    Href = "/dashboard",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(DashboardRazorResources.Menu.WidgetGroups)
                {
                    Name = "Widget groups",
                    ParentMenuItemId = DashboardRazorResources.Menu.DashboardConfiguration,
                    Href = "/WidgetGroup",
                    Translate = "system_widget_groups",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(DashboardRazorResources.Menu.Widgets)
                {
                    Name = "Widgets",
                    ParentMenuItemId = DashboardRazorResources.Menu.DashboardConfiguration,
                    Href = "/Widget",
                    Translate = "system_widgets",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                }
            }
        };
    }
}
