using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Identity.Razor.Helpers
{
    internal class IdentityMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = IdentityRazorResources.Menu.People,
                    ParentMenuItemId = MenuResources.MenuItems.AdministrationItem,
                    Name = "People",
                    Translate = "people",
                    Href = "#"
                },
                new MenuItemConfig
                {
                    Id = IdentityRazorResources.Menu.Users,
                    Name = "Users",
                    ParentMenuItemId = IdentityRazorResources.Menu.People,
                    Href = "/Users",
                    Translate = "users"
                },
                new MenuItemConfig
                {
                    Id = IdentityRazorResources.Menu.Roles,
                    Name = "Roles",
                    Href = "/Roles",
                    ParentMenuItemId = IdentityRazorResources.Menu.People,
                    Translate = "roles"
                },
                new MenuItemConfig
                {
                    Id = IdentityRazorResources.Menu.Groups,
                    Name = "Groups",
                    ParentMenuItemId = IdentityRazorResources.Menu.People,
                    Href = "/Groups",
                    Translate = "groups"
                }
            }
        };
    }
}
