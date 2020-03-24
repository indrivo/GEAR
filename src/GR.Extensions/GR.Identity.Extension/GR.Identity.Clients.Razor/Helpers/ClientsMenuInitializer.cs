using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Identity.Clients.Razor.Helpers
{
    public sealed class ClientsMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig(ApiClientsResources.Menu.ClientsSection)
                {
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Name = "API Clients",
                    Href = MenuResources.MenuItems.None
                },
                new MenuItemConfig(ApiClientsResources.Menu.Clients)
                {
                    ParentMenuItemId = ApiClientsResources.Menu.ClientsSection,
                    Name = "Clients",
                    Href = "/Clients"
                },
                new MenuItemConfig(ApiClientsResources.Menu.ClientResources)
                {
                    ParentMenuItemId = ApiClientsResources.Menu.ClientsSection,
                    Name = "Resources",
                    Href = "/ClientResources"
                }
            }
        };
    }
}
