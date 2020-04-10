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
                new MenuItemConfig(DocumentRazorResources.Menu.DocumentSectionMenuItem)
                {
                    ParentMenuItemId = MenuResources.MenuItems.ConfigurationItem,
                    Name = "Documents Configuration",
                    Href =MenuResources.MenuItems.None
                },

                new MenuItemConfig(DocumentRazorResources.Menu.DocumentTypeMenuItem)
                {
                    ParentMenuItemId = DocumentRazorResources.Menu.DocumentSectionMenuItem,
                    Name = "Document types",
                    Translate = "system_document_types",
                    Href = "/DocumentTypes"
                },

                new MenuItemConfig(DocumentRazorResources.Menu.DocumentCategoryMenuItem)
                {
                    ParentMenuItemId = DocumentRazorResources.Menu.DocumentSectionMenuItem,
                    Name = "Document category",
                    Translate = "system_document_categories",
                    Href ="/DocumentCategories"
                },

                new MenuItemConfig(DocumentRazorResources.Menu.DocumentMenuItem)
                {
                    ParentMenuItemId = MenuResources.MenuItems.AppsItem,
                    Name = "Documents",
                    Translate = "system_documents",
                    Href ="/Documents"
                },
            }
        };
    }
}
