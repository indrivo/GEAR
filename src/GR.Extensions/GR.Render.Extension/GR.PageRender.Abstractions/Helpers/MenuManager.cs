using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;
using Mapster;
using Microsoft.EntityFrameworkCore.Internal;

namespace GR.PageRender.Abstractions.Helpers
{
    public static class MenuManager
    {
        /// <summary>
        /// Navbar id
        /// </summary>
        public static Guid NavBarId => Guid.Parse("46EACBA3-D515-47B0-9BA7-5391CE1D26B1".ToLower());

        /// <summary>
        /// Dynamic service
        /// </summary>
        private static IDynamicService DynamicService => IoC.Resolve<IDynamicService>();
        /// <summary>
        /// List of menus
        /// </summary>
        public static List<Menu> Menu = new List<Menu>
        {
            new Menu
            {
                Id = NavBarId,
                Name = "Main Navbar",
                Description = "Default navbar for website",
                Author = "System",
                Created = DateTime.Now
            }
        };

        /// <summary>
        /// Sync default menus
        /// </summary>
        public static async Task SyncMenuItemsAsync()
        {
            if (DynamicService == null) throw new Exception("IDynamicService is not registered");
            var exists = await DynamicService.Any<Menu>();
            if (exists.Result) return;
            var rq = await DynamicService.AddDataRangeWithReflection(Menu);
            if (rq.Result.All(x => x.Item2 != null))
            {
                var menus = GetMenus().ToList();
                foreach (var item in menus)
                {
                    item.Order = menus.IndexOf(item);
                    await SyncMenuItem(item);
                }
            }
        }

        /// <summary>
        /// Sync menu item
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        private static async Task SyncMenuItem(MenuViewModel menu)
        {
            var res = await DynamicService.AddWithReflection(menu.Adapt<MenuItem>());
            if (!res.IsSuccess || menu.Children == null) return;
            foreach (var item in menu.Children)
            {
                item.ParentMenuItemId = res.Result;
                item.Order = menu.Children.IndexOf(item);
                await SyncMenuItem(item);
            }
        }

        /// <summary>
        /// Read menus
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<MenuViewModel> GetMenus()
            => JsonParser.ReadArrayDataFromJsonFile<List<MenuViewModel>>(Path.Combine(AppContext.BaseDirectory, "Configuration/menus.json"));
    }
}
