using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ST.Cache.Abstractions;
using ST.Core;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.PageRender.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;

namespace ST.PageRender.Razor.Services
{
    public static class MenuHelper
    {
        /// <summary>
        /// Get menu cache key
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public static string GetCacheKey(string menuId)
        {
            return $"_menu_{menuId}";
        }
    }

    public class MenuService<TService> : IMenuService where TService : IDynamicService
    {
        #region Injectable

        /// <summary>
        /// Inject Data Service
        /// </summary>
        private readonly TService _service;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="cacheService"></param>
        public MenuService(TService service, ICacheService cacheService)
        {
            _service = service;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Get menus
        /// </summary>
        /// <param name="menuBlockId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<MenuItem>> GetSourceAsync(Guid menuBlockId, Guid menuId)
        {
            var cache = await _cacheService.Get<List<MenuItem>>(MenuHelper.GetCacheKey(menuId.ToString()));
            if (cache != null && cache.Any()) return cache;
            var search = await _service.GetAll<MenuItem, MenuItem>(x => x.MenuId.Equals(menuBlockId));
            await _cacheService.Set(MenuHelper.GetCacheKey(menuId.ToString()), search.Result);
            return search.Result.ToList();
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
            var navbar = await _service.GetByIdWithReflection<Menu, Menu>(menuId.Value);
            if (!navbar.IsSuccess) return result;
            var menus = (await GetSourceAsync(navbar.Result.Id, menuId.Value))
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
            var match = await _service.GetAllWithInclude<MenuItem, MenuItem>(null, new List<Filter>
            {
                new Filter(nameof(BaseModel.Id), menuId)
            });
            var menu = match.Result?.FirstOrDefault();
            if (menu == null) return default;
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

        /// <inheritdoc />
        /// <summary>
        /// Update menu item role access
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> UpdateMenuItemRoleAccess(Guid menuId, IList<string> roles)
        {
            var match = await _service.GetAllWithInclude<MenuItem, MenuItem>(null, new List<Filter>
            {
                new Filter(nameof(BaseModel.Id), menuId)
            });
            var menu = match.Result?.FirstOrDefault();
            if (!match.IsSuccess || menu == null) return new ResultModel<Guid>();
            if (!roles.Contains(GlobalResources.Roles.ADMINISTRATOR)) roles.Add(GlobalResources.Roles.ADMINISTRATOR);
            menu.AllowedRoles = string.Join("#", roles);
            await _cacheService.RemoveAsync(MenuHelper.GetCacheKey(menu.MenuId.ToString()));
            return await _service.UpdateWithReflection(menu);
        }
    }
}
