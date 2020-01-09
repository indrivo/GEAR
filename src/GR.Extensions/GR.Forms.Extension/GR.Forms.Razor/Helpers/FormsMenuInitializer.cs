using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Forms.Razor.Helpers
{
    internal class FormsMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = FormsRazorResources.Menu.FormsItem,
                    Name = "Forms",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Href = "/Form",
                    Translate = "forms"
                }
            }
        };
    }
}
