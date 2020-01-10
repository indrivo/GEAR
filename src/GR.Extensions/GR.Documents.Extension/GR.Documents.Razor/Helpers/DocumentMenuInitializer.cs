using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Documents.Razor.Helpers
{
    internal class DocumentMenuInitializer : BaseMenuInitializer
    {

        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {

                new MenuItemConfig(DocumentRazorResources.Menu.DocumentMenuItem)
                {
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Name = "Documents",
                    Translate = "system_documents",
                    Href ="/Documents"
                },

                new MenuItemConfig(DocumentRazorResources.Menu.DocumentTypeMenuItem)
                {
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Name = "Document types",
                    Translate = "system_document_types",
                    Href = "/DocumentTypes"
                },

                new MenuItemConfig(DocumentRazorResources.Menu.DocumentCategoryMenuItem)
                {
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Name = "Document category",
                    Translate = "system_document_categories",
                    Href ="/DocumentCategories"
                },
            }
        };
    }
}
