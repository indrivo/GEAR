﻿using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Helpers.Icons;

namespace GR.UI.Menu.Helpers
{
    public class AppBaseMenuInitializer : BaseMenuInitializer
    {
        /// <summary>
        /// Config for menu items
        /// </summary>
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = MenuResources.MenuItems.HomeItem,
                    Name = "Dashboard",
                    Href = "/home",
                    Translate = "dashboard",
                    Icon = FontAwesomeIcons.FA_DASHBOARD
                },
                new MenuItemConfig
                {
                    Id = MenuResources.MenuItems.AdministrationItem,
                    Name = "Administration",
                    Href = "#",
                    Translate = "administration",
                    Icon = FontAwesomeIcons.FA_GEARS
                },
                new MenuItemConfig
                {
                    Id = MenuResources.MenuItems.ConfigurationItem,
                    ParentMenuItemId = MenuResources.MenuItems.AdministrationItem,
                    Name = "Configuration",
                    Href = "#",
                    Translate = "configuration",
                },
                new MenuItemConfig
                {
                    Id = MenuResources.MenuItems.AppsItem,
                    Name = "Apps",
                    Href = "#",
                    Translate = "apps",
                    Icon = FontAwesomeIcons.FA_CIRCLE_O_NOTCH
                }
            }
        };
    }
}
