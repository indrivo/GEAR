using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Localization.Razor.Helpers
{
    internal class LocalizationMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = LocalizationRazorResources.Menu.LocalizationItem,
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Name = "Localization",
                    Translate = "localization",
                    Href = MenuResources.MenuItems.None
                },
                new MenuItemConfig
                {
                    Id = LocalizationRazorResources.Menu.LanguagesItem,
                    ParentMenuItemId = LocalizationRazorResources.Menu.LocalizationItem,
                    Name = "Languages",
                    Translate = "languages",
                    Href = "/Localization/GetLanguages"
                },
                new MenuItemConfig
                {
                    Id = LocalizationRazorResources.Menu.KeysItem,
                    ParentMenuItemId = LocalizationRazorResources.Menu.LocalizationItem,
                    Name = "Keys",
                    Href = "/Localization",
                    Translate = "keys"
                }
            }
        };
    }
}
