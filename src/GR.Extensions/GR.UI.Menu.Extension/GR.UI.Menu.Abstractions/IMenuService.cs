using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.UI.Menu.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Models;
using GR.UI.Menu.Abstractions.ViewModels;

namespace GR.UI.Menu.Abstractions
{
    public interface IMenuService
    {
        /// <summary>
        /// Get menus
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<MenuViewModel>>> GetMenus(Guid? menuId, IList<string> roles);

        /// <summary>
        /// Get Menu Roles
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetMenuRoles(Guid menuId);

        /// <summary>
        /// Update menu item role access
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> UpdateMenuItemRoleAccess(Guid menuId, IList<string> roles);

        /// <summary>
        /// Have Access to view menu
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="menuItemAllowedRoles"></param>
        /// <returns></returns>
        bool HaveAccess(IList<string> userRoles, string menuItemAllowedRoles);

        /// <summary>
        /// Create menu group
        /// </summary>
        /// <param name="menuGroup"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateMenuGroupAsync(MenuGroup menuGroup);

        /// <summary>
        /// Create menu item
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> CreateMenuItemAsync(MenuItem menuItem, IEnumerable<string> roles = null);

        /// <summary>
        /// Find menu group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<MenuGroup>> FindMenuGroupByIdAsync(Guid? id);

        /// <summary>
        /// Get menu item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<MenuItem>> FindMenuItemByIdAsync(Guid? id);

        /// <summary>
        /// Update menu group
        /// </summary>
        /// <param name="menuGroup"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateMenuGroupAsync(MenuGroup menuGroup);

        /// <summary>
        /// Update menu item
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateMenuItemAsync(MenuItem menuItem);

        /// <summary>
        /// Get menu items
        /// </summary>
        /// <param name="param"></param>
        /// <param name="menuId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        DTResult<MenuItem> GetPaginatedMenuItems(DTParameters param, Guid menuId, Guid? parentId = null);

        /// <summary>
        /// Get paginated menu groups
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        DTResult<MenuGroup> GetPaginatedMenuGroups(DTParameters param);

        /// <summary>
        /// Delete menu item
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteMenuItemAsync(Guid? menuItemId);

        /// <summary>
        /// Delete menu group
        /// </summary>
        /// <param name="menuGroupId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteMenuGroupAsync(Guid? menuGroupId);

        /// <summary>
        /// Get childs
        /// </summary>
        /// <param name="parentMenuItemId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<MenuItem>>> GetChildsOfMenuItemAsync(Guid? parentMenuItemId);

        /// <summary>
        /// Append new menu
        /// </summary>
        /// <typeparam name="TMenuInitializer"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        Task<ResultModel> AppendMenuItemsAsync<TMenuInitializer>(TMenuInitializer initializer)
            where TMenuInitializer : BaseMenuInitializer;
    }
}