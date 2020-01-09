using System;
using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Entities.Security.Razor.Helpers
{
    internal class EntitiesPermissionsMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = EntitiesPermissionsRazorResources.Menu.EntitiesRoleAccessItem,
                    ParentMenuItemId =  Guid.Parse("c852a3d6-a2ac-4f91-aed5-9b8f11a9b453"),
                    Name = "Entities Role Access",
                    Href = "/EntitySecurity",
                    Translate = "none"
                }
            }
        };
    }
}
