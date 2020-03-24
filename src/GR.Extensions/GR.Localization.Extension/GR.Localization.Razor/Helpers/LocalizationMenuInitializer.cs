using System.Collections.Generic;
using GR.Core;
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
                    Href = MenuResources.MenuItems.None,
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig
                {
                    Id = LocalizationRazorResources.Menu.LanguagesItem,
                    ParentMenuItemId = LocalizationRazorResources.Menu.LocalizationItem,
                    Name = "Languages",
                    Translate = "languages",
                    Href = "/Localization/GetLanguages",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig
                {
                    Id = LocalizationRazorResources.Menu.KeysItem,
                    ParentMenuItemId = LocalizationRazorResources.Menu.LocalizationItem,
                    Name = "Keys",
                    Href = "/Localization",
                    Translate = "keys",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(LocalizationRazorResources.Menu.Countries)
                {
                    ParentMenuItemId = LocalizationRazorResources.Menu.LocalizationItem,
                    Name = "Countries",
                    Href = "/Countries",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                }
            }
        };
    }
}
