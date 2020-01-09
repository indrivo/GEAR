using GR.UI.Menu.Abstractions.Helpers;
using System.Collections.Generic;

namespace GR.PageRender.Razor.Helpers
{
    internal class PagesMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = PagesRazorResources.Menu.PagesMenuItem,
                    Name = "Pages",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Translate = "pages",
                    Href = "/Page"
                },
                new MenuItemConfig
                {
                    Id = PagesRazorResources.Menu.LayoutsMenuItem,
                    Name = "Layouts",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Translate = "layouts",
                    Href = "/Page/Layouts"
                },
                new MenuItemConfig
                {
                    Id = PagesRazorResources.Menu.PageTypesMenuItem,
                    Name = "Page Types",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Translate = "page_types",
                    Href = "/PageType"
                },
                new MenuItemConfig
                {
                    Id = PagesRazorResources.Menu.RenderBlocksMenuItem,
                    Name = "Render Blocks",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Translate = "render_blocks",
                    Href = "/Blocks"
                },
                new MenuItemConfig
                {
                    Id = PagesRazorResources.Menu.RenderBlocksCategoriesMenuItem,
                    Name = "Render Block categories",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Translate = "render_blocks_categories",
                    Href = "/BlockCategory"
                },
                new MenuItemConfig
                {
                    Id = PagesRazorResources.Menu.TemplatesMenuItem,
                    Name = "Templates",
                    ParentMenuItemId = MenuResources.MenuItems.AppearanceMenuItem,
                    Translate = "templates",
                    Href = "/templates"
                }
            }
        };
    }
}
