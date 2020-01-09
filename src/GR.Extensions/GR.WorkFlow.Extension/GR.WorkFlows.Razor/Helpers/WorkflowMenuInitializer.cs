using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.WorkFlows.Razor.Helpers
{
    internal class WorkflowMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = WorkflowRazorResources.Menu.WorkflowBuilderItem,
                    Name = "Workflow Builder",
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Href = "/WorkflowBuilder"
                }
            }
        };
    }
}
