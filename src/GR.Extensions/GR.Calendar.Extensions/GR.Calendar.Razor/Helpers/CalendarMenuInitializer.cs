using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Calendar.Razor.Helpers
{
    internal class CalendarMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder =>
            new MenuInitBuilder
            {
                MenuGroup = MenuResources.AppMenuId,
                Configs = new List<MenuItemConfig>
                {
                    new MenuItemConfig
                    {
                        Id = CalendarRazorResources.Menu.Calendar,
                        Name = "Calendar",
                        Translate = "system_calendar",
                        ParentMenuItemId = MenuResources.MenuItems.AppsItem,
                        Href = "/calendar"
                    }
                }
            };
    }
}
