using System;
using System.Collections.Generic;
using GR.MultiTenant.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Helpers.Icons;

namespace GR.MultiTenant.Razor.Helpers
{
    internal class MultiTenantMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig
                {
                    Id = MultiTenantRazorResources.Menu.CompanySettings,
                    Icon = FontAwesomeIcons.FA_BUILDING,
                    Name = "Company Settings",
                    Translate = "iso_menu_company_settings",
                    Href = "#",
                    AllowedRoles = new List<string>
                    {
                        MultiTenantResources.Roles.COMPANY_ADMINISTRATOR
                    }
                },
                new MenuItemConfig
                {
                    Id = MultiTenantRazorResources.Menu.CompanyInformation,
                    Name = "Company Information",
                    Translate = "system_menu_company_info",
                    Href = "/CompanyManage",
                    ParentMenuItemId = MultiTenantRazorResources.Menu.CompanySettings,
                    AllowedRoles = new List<string>
                    {
                        MultiTenantResources.Roles.COMPANY_ADMINISTRATOR
                    }
                },
                new MenuItemConfig
                {
                    Id = MultiTenantRazorResources.Menu.Tenants,
                    ParentMenuItemId = Guid.Parse("c3cd3495-5ee1-4fbb-b8d5-9d792ac7104b"),
                    Name = "Tenants",
                    Href = "/Tenant",
                    Translate = "tenants"
                }
            }
        };
    }
}
