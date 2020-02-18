using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Models;
using GR.UI.Menu.Abstractions.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GR.UI.Menu
{
    public class MenuService : IMenuService
    {
        #region Injectable

        /// <summary>
        /// Inject Data Service
        /// </summary>
        private readonly IMenuDbContext _context;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly IMemoryCache _cacheService;

        /// <summary>
        /// Inject filter
        /// </summary>
        private readonly IDataFilter _filter;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheService"></param>
        /// <param name="filter"></param>
        public MenuService(IMenuDbContext context, IMemoryCache cacheService, IDataFilter filter)
        {
            _context = context;
            _cacheService = cacheService;
            _filter = filter;
        }

        /// <summary>
        /// Get menus
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<MenuItem>> GetMenuBlockAsync(Guid menuId)
        {
            var cache = _cacheService.Get<List<MenuItem>>(MenuHelper.GetCacheKey(menuId.ToString()));
            if (cache != null && cache.Any()) return cache;
            var search = await _context.MenuItems.Where(x => x.MenuId.Equals(menuId)).ToListAsync();
            if (search.Any())
            {
                _cacheService.Set(MenuHelper.GetCacheKey(menuId.ToString()), search);
            }

            return search.ToList();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get menus
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<MenuViewModel>>> GetMenus(Guid? menuId, IList<string> roles)
        {
            var result = new ResultModel<IEnumerable<MenuViewModel>>();
            if (!menuId.HasValue) return result;
            //var navbar = await _cacheService.Get<Menu>(MenuHelper.GetBlockCacheKey(menuId.ToString()));
            //if (navbar == null)
            //{
            //    var dbNavbar = await _service.GetByIdWithReflection<Menu, Menu>(menuId.Value);
            //    if (!dbNavbar.IsSuccess) return result;
            //    navbar = dbNavbar.Result;
            //    await _cacheService.Set(MenuHelper.GetBlockCacheKey(menuId.ToString()), navbar);
            //}

            var menus = (await GetMenuBlockAsync(menuId.Value))
                .Where(x => HaveAccess(roles, x.AllowedRoles))
                .ToList();
            if (!menus.Any())
            {
                result.Errors.Add(new ErrorModel(nameof(EmptyResult), "No menu are available for you!"));
                return result;
            }

            result.IsSuccess = true;
            result.Result = GetMenu(menus, roles).OrderBy(x => x.Order).ToList();
            return result;
        }

        /// <summary>
        /// Get recursive menus
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="roles"></param>
        /// <param name="parentMenuId"></param>
        /// <returns></returns>
        protected virtual IEnumerable<MenuViewModel> GetMenu(IList<MenuItem> menus, IList<string> roles, Guid? parentMenuId = null)
        {
            var data = menus
                .Where(x => x.ParentMenuItemId == parentMenuId && HaveAccess(roles, x.AllowedRoles))
                .Adapt<IEnumerable<MenuViewModel>>().ToList();

            foreach (var t in data)
            {
                t.Children = GetMenu(menus, roles, t.Id).OrderBy(x => x.Order).ToArray();
            }
            return data;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get menu roles
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<string>> GetMenuRoles(Guid menuId)
        {
            var menuItemRequest = await FindMenuItemByIdAsync(menuId);
            if (!menuItemRequest.IsSuccess) return new List<string>();
            var menu = menuItemRequest.Result;
            var preRoles = menu.AllowedRoles;
            if (string.IsNullOrEmpty(preRoles)) return default;
            var roles = preRoles.Split('#').ToList();
            return roles;
        }

        /// <inheritdoc />
        /// <summary>
        /// Have user access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="menuItemAllowedRoles"></param>
        /// <returns></returns>
        public virtual bool HaveAccess(IList<string> userRoles, string menuItemAllowedRoles)
        {
            if (string.IsNullOrEmpty(menuItemAllowedRoles)) return false;
            if (!userRoles.Any() || !userRoles.Contains(GlobalResources.Roles.ANONIMOUS_USER))
            {
                userRoles.Add(GlobalResources.Roles.ANONIMOUS_USER);
            }

            try
            {
                var menuItemRoles = menuItemAllowedRoles.Split('#');
                if (userRoles.Intersect(menuItemRoles).Any()) return true;
            }
            catch
            {
                //Ignore
            }
            return false;
        }

        /// <summary>
        /// Create menu group
        /// </summary>
        /// <param name="menuGroup"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> CreateMenuGroupAsync(MenuGroup menuGroup)
        {
            if (menuGroup == null) return new InvalidParametersResultModel().Map<Guid>();
            await _context.Menus.AddAsync(menuGroup);
            var dbRequest = await _context.PushAsync();
            return !dbRequest.IsSuccess ? dbRequest.Map<Guid>() : new SuccessResultModel<Guid>(menuGroup.Id);
        }

        /// <summary>
        /// Create menu item
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> CreateMenuItemAsync(MenuItem menuItem, IEnumerable<string> roles = null)
        {
            if (menuItem == null) return new InvalidParametersResultModel().Map<Guid>();
            menuItem.AllowedRoles = $"{GlobalResources.Roles.ADMINISTRATOR}#";

            foreach (var role in roles ?? new List<string>())
            {
                if (role.Equals(GlobalResources.Roles.ADMINISTRATOR)) continue;
                menuItem.AllowedRoles += $"{role}#";
            }

            var data = await _context.MenuItems.Where(x =>
                x.ParentMenuItemId == menuItem.ParentMenuItemId).ToListAsync();
            menuItem.Order = data.Any() ? data.Max(x => x.Order) + 1 : 1;
            await _context.MenuItems.AddAsync(menuItem);
            var dbRequest = await _context.PushAsync();
            if (!dbRequest.IsSuccess) return dbRequest.Map<Guid>();
            if (dbRequest.IsSuccess) _cacheService.Remove(MenuHelper.GetCacheKey(menuItem.MenuId.ToString()));
            return new SuccessResultModel<Guid>(menuItem.Id);
        }

        /// <summary>
        /// Find menu group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultModel<MenuGroup>> FindMenuGroupByIdAsync(Guid? id)
        {
            if (id == null) return new InvalidParametersResultModel<MenuGroup>();
            var menuGroup = await _context.Menus
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (menuGroup == null) return new NotFoundResultModel<MenuGroup>();
            return new SuccessResultModel<MenuGroup>(menuGroup);
        }

        /// <summary>
        /// Find menu item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultModel<MenuItem>> FindMenuItemByIdAsync(Guid? id)
        {
            if (id == null) return new InvalidParametersResultModel<MenuItem>();
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (menuItem == null) return new NotFoundResultModel<MenuItem>();
            return new SuccessResultModel<MenuItem>(menuItem);
        }

        /// <summary>
        /// Update menu group
        /// </summary>
        /// <param name="menuGroup"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateMenuGroupAsync(MenuGroup menuGroup)
        {
            if (menuGroup == null) return new InvalidParametersResultModel();
            var menuGroupRequest = await FindMenuGroupByIdAsync(menuGroup.Id);
            if (!menuGroupRequest.IsSuccess) return menuGroupRequest.ToBase();
            var menu = menuGroupRequest.Result;
            menu.Name = menuGroup.Name;
            menu.Description = menuGroup.Description;
            _context.Update(menu);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Update menu item
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateMenuItemAsync(MenuItem menuItem)
        {
            if (menuItem == null) return new InvalidParametersResultModel();
            var menuItemRequest = await FindMenuItemByIdAsync(menuItem.Id);
            if (!menuItemRequest.IsSuccess) return menuItemRequest.ToBase();
            var menu = menuItemRequest.Result;
            menu.Name = menuItem.Name;
            menu.Href = menuItem.Href;
            menu.Icon = menuItem.Icon;
            menu.Order = menuItem.Order;
            menu.Translate = menuItem.Translate;
            menu.AllowedRoles = menuItem.AllowedRoles;
            _context.MenuItems.Update(menu);
            var dbRequest = await _context.PushAsync();
            if (dbRequest.IsSuccess) _cacheService.Remove(MenuHelper.GetCacheKey(menuItem.MenuId.ToString()));
            return dbRequest;
        }

        /// <summary>
        /// Get menu items in pagination format
        /// </summary>
        /// <param name="param"></param>
        /// <param name="menuId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public DTResult<MenuItem> GetPaginatedMenuItems(DTParameters param, Guid menuId, Guid? parentId = null)
        {
            var data = _filter.FilterAbstractEntity<MenuItem, IMenuDbContext>(_context, param.Search.Value,
                               param.SortOrder, param.Start,
                               param.Length, out var totalCount,
                               x => x.MenuId.Equals(menuId) && x.ParentMenuItemId.Equals(parentId))
                           ?.ToList() ?? new List<MenuItem>();

            var finalResult = new DTResult<MenuItem>
            {
                Draw = param.Draw,
                Data = data.OrderBy(x => x.Order).ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = data.Count()
            };

            return finalResult;
        }

        /// <summary>
        /// Get menu groups on paginated format
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public DTResult<MenuGroup> GetPaginatedMenuGroups(DTParameters param)
        {
            var data = _filter.FilterAbstractEntity<MenuGroup, IMenuDbContext>(_context, param.Search.Value,
                               param.SortOrder, param.Start,
                               param.Length, out var totalCount)
                           ?.ToList() ?? new List<MenuGroup>();

            var finalResult = new DTResult<MenuGroup>
            {
                Draw = param.Draw,
                Data = data.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = data.Count()
            };

            return finalResult;
        }

        /// <summary>
        /// Delete menu item
        /// </summary>
        /// <param name="menuItemId"></param>
        /// <returns></returns>
        public Task<ResultModel> DeleteMenuItemAsync(Guid? menuItemId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete menu group
        /// </summary>
        /// <param name="menuGroupId"></param>
        /// <returns></returns>
        public Task<ResultModel> DeleteMenuGroupAsync(Guid? menuGroupId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get childs of menu item
        /// </summary>
        /// <param name="parentMenuItemId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<MenuItem>>> GetChildsOfMenuItemAsync(Guid? parentMenuItemId)
        {
            var childs = await _context.MenuItems
                .Include(x => x.ParentMenuItem)
                .Where(x => x.ParentMenuItemId.Equals(parentMenuItemId))
                .ToListAsync();
            return new SuccessResultModel<IEnumerable<MenuItem>>(childs);
        }

        /// <inheritdoc />
        /// <summary>
        /// Update menu item role access
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> UpdateMenuItemRoleAccess(Guid menuId, IList<string> roles)
        {
            var menuItemRequest = await FindMenuItemByIdAsync(menuId);
            if (!menuItemRequest.IsSuccess) return menuItemRequest.Map<Guid>();
            var menu = menuItemRequest.Result;

            if (!roles.Contains(GlobalResources.Roles.ADMINISTRATOR)) roles.Add(GlobalResources.Roles.ADMINISTRATOR);
            menu.AllowedRoles = string.Join("#", roles);
            _cacheService.Remove(MenuHelper.GetCacheKey(menu.MenuId.ToString()));
            var updateRequest = await UpdateMenuItemAsync(menu);
            return updateRequest.IsSuccess ? new SuccessResultModel<Guid>(menuId) : updateRequest.Map<Guid>();
        }

        /// <summary>
        /// Append menu items
        /// </summary>
        /// <typeparam name="TMenuInitializer"></typeparam>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AppendMenuItemsAsync<TMenuInitializer>(TMenuInitializer initializer)
            where TMenuInitializer : BaseMenuInitializer
        {
            return await initializer.ExecuteAsync();
        }
    }
}
