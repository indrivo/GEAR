using System;
using System.Threading.Tasks;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Events;
using GR.UI.Menu.Abstractions.Events.EventArgs;
using GR.UI.Menu.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Models;
using GR.UI.Menu.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GR.UI.Menu.Data
{
    public static class MenuDbContextSeeder
    {
        /// <summary>
        /// Seed 
        /// </summary>
        /// <returns></returns>
        public static async Task SeedAsync(IServiceProvider services)
        {
            var menuService = services.GetRequiredService<IMenuService>();
            var checkExist = await menuService.FindMenuGroupByIdAsync(MenuResources.AppMenuId);
            if (checkExist.IsSuccess) return;
            var serviceRequest = await menuService.CreateMenuGroupAsync(new MenuGroup
            {
                Id = MenuResources.AppMenuId,
                Name = "Main app menu",
                Description = "Default menu for app",
                Author = "System",
                Created = DateTime.Now
            });

            if (serviceRequest.IsSuccess) await menuService.AppendMenuItemsAsync(new AppBaseMenuInitializer());

            //Trigger seed menus
            MenuEvents.Menu.MenuSeed(new MenuSeedEventArgs());
        }
    }
}
