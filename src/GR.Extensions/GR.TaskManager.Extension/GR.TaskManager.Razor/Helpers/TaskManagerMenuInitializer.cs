using System;
using System.Collections.Generic;
using System.Text;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.TaskManager.Razor.Helpers
{
    public class TaskManagerMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = TaskRazorResources.Menu.TaskManager,
                    Name = "Task Manager",
                    ParentMenuItemId = MenuResources.MenuItems.AppsItem,
                    Translate = "none",
                    Href = "/TaskManager"
                }
            }
        };
    }
}
