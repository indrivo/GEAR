using System.Collections.Generic;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.Report.Dynamic.Razor.Helpers
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class ReportsMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig(ReportRazorResources.Menu.ReportsSection)
                {
                    Name = "Reports",
                    ParentMenuItemId = MenuResources.MenuItems.AdministrationItem,
                    Href = MenuResources.MenuItems.None,
                    Translate = "reports",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(ReportRazorResources.Menu.Reports)
                {
                    Name = "Reports",
                    ParentMenuItemId = ReportRazorResources.Menu.ReportsSection,
                    Href = "/Reports",
                    Translate = "reports",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                },
                new MenuItemConfig(ReportRazorResources.Menu.Folders)
                {
                    Name = "Report folders",
                    ParentMenuItemId = ReportRazorResources.Menu.ReportsSection,
                    Href = "/Reports/ManageDynamicReportFolders",
                    AllowedRoles = new List<string>
                    {
                        GlobalResources.Roles.ADMINISTRATOR
                    }
                }
            }
        };
    }
}