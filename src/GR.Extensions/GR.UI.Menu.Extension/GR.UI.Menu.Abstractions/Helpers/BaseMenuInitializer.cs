using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.UI.Menu.Abstractions.Models;

namespace GR.UI.Menu.Abstractions.Helpers
{
    public abstract class BaseMenuInitializer
    {
        private static IMenuService _service;
        protected IMenuService MenuService => _service ?? (_service = IoC.Resolve<IMenuService>());

        /// <summary>
        /// Builder
        /// </summary>
        public abstract MenuInitBuilder Builder { get; }


        /// <summary>
        /// Execute seed
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> ExecuteAsync()
        {
            var responses = new List<ResultModel<Guid>>();
            foreach (var item in Builder.Configs)
            {
                var menuItem = new MenuItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Href = item.Href,
                    Icon = item.Icon,
                    ParentMenuItemId = item.ParentMenuItemId,
                    MenuId = Builder.MenuGroup,
                    Order = item.Order,
                    Translate = item.Translate
                };

                var exist = await MenuService.FindMenuItemByIdAsync(menuItem.Id);
                if (exist.IsSuccess) continue;
                var itemResponse = await MenuService.CreateMenuItemAsync(menuItem, item.AllowedRoles);
                responses.Add(itemResponse);
            }

            return new ResultModel
            {
                IsSuccess = responses.TrueForAll(x => x.IsSuccess)
            };
        }
    }

    public class MenuItemConfig
    {
        public MenuItemConfig() { }

        public MenuItemConfig(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Menu id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Href
        /// </summary>
        public virtual string Href { get; set; } = "#";

        /// <summary>
        /// Translate key
        /// </summary>
        public virtual string Translate { get; set; } = "none";

        /// <summary>
        /// Menu item icon
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// Parent menu item
        /// </summary>
        public virtual Guid? ParentMenuItemId { get; set; } = null;

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; } = 1;

        /// <summary>
        /// Allowed roles
        /// </summary>
        public IEnumerable<string> AllowedRoles { get; set; } = new List<string>();
    }

    public class MenuInitBuilder
    {
        /// <summary>
        /// Configs
        /// </summary>
        public IList<MenuItemConfig> Configs { get; set; } = new List<MenuItemConfig>();

        /// <summary>
        /// Menu
        /// </summary>
        public Guid MenuGroup { get; set; } = MenuResources.AppMenuId;
    }
}
