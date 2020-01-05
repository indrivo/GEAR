using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Entities.Razor.Helpers
{
    internal class EntitiesMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = EntitiesRazorResources.Menu.StructureItem,
                    Name = "Structure",
                    ParentMenuItemId = MenuResources.MenuItems.AdministrationItem,
                    Translate = "structure",
                    Href = MenuResources.MenuItems.None
                },
                new MenuItemConfig
                {
                    Id = EntitiesRazorResources.Menu.EntitiesItem,
                    ParentMenuItemId = EntitiesRazorResources.Menu.StructureItem,
                    Name = "Entities",
                    Href = MenuResources.MenuItems.None,
                    Translate = "entities"
                },
                new MenuItemConfig
                {
                    Id = EntitiesRazorResources.Menu.TablesItem,
                    ParentMenuItemId = EntitiesRazorResources.Menu.EntitiesItem,
                    Name = "Tables",
                    Href = "/Table",
                    Translate = "tables"
                },
                new MenuItemConfig
                {
                    Id = EntitiesRazorResources.Menu.SchemesItem,
                    Name = "Schemes",
                    ParentMenuItemId = EntitiesRazorResources.Menu.EntitiesItem,
                    Href = "/EntityType",
                    Translate = "system_schemes"
                }
            }
        };
    }
}
