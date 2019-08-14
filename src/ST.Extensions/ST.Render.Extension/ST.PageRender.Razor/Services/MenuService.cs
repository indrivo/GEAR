using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using ST.Cache.Abstractions;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Enums;
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
        /// <summary>
        /// Inject Data Service
        /// </summary>
        private readonly TService _service;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

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

        /// <inheritdoc />
        /// <summary>
        /// Get menus
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<MenuViewModel>>> GetMenus(Guid? menuId, IList<string> roles)
        {
            if (!menuId.HasValue) return default;
            var navbar = await _service.GetByIdWithReflection<Menu, Menu>(menuId.Value);
            if (!navbar.IsSuccess) return default;
            List<MenuItem> menus;
            var cache = await _cacheService.Get<List<MenuItem>>(MenuHelper.GetCacheKey(menuId.ToString()));
            if (cache == null || cache.Count == 0)
            {
                var search = await _service.GetAll<MenuItem, MenuItem>(x => x.MenuId.Equals(navbar.Result.Id));
                menus = search.Result.ToList();
                await _cacheService.Set(MenuHelper.GetCacheKey(menuId.ToString()), search.Result);
            }
            else
            {
                menus = cache;
            }

            if (!menus.Any()) return new ResultModel<IEnumerable<MenuViewModel>>
            {
                Errors = new List<IErrorModel>
                {
                    new ErrorModel("Null", "No menu are available for you!")
                }
            };
            menus = menus.Where(x => HaveAccess(roles, x.AllowedRoles)).ToList();
            return new ResultModel<IEnumerable<MenuViewModel>>
            {
                IsSuccess = true,
                Result = GetMenu(menus, roles).OrderBy(x => x.Order).ToList()
            };
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
                new Filter
                {
                    Value = menuId,
                    Criteria = Criteria.Equals,
                    Parameter = "Id"
                }
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
            var anonymousRole = "Anonymous User";
            if (string.IsNullOrEmpty(menuItemAllowedRoles)) return false;
            if (!userRoles.Any() || !userRoles.Contains(anonymousRole))
            {
                userRoles.Add(anonymousRole);
            }

            try
            {
                var menuItemRoles = menuItemAllowedRoles.Split('#');
                if (userRoles.Intersect(menuItemRoles).Any()) return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                new Filter
                {
                    Value = menuId,
                    Criteria = Criteria.Equals,
                    Parameter = "Id"
                }
            });
            var menu = match.Result?.FirstOrDefault();
            if (!match.IsSuccess || menu == null) return new ResultModel<Guid>();
            if (!roles.Contains("Administrator")) roles.Add("Administrator");
            menu.AllowedRoles = string.Join("#", roles);
            await _cacheService.RemoveAsync(MenuHelper.GetCacheKey(menu.MenuId.ToString()));
            return await _service.UpdateWithReflection(menu);
        }
    }
}
