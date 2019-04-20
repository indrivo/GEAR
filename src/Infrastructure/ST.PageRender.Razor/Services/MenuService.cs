using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Models.Pages;
using ST.Identity.Services.Abstractions;
using ST.PageRender.Razor.Services.Abstractions;

namespace ST.PageRender.Razor.Services
{
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
        public async Task<ResultModel<IEnumerable<MenuViewModel>>> GetMenus(Guid? menuId, IList<string> roles)
        {
            if (!menuId.HasValue) return default;
            var navbar = await _service.GetByIdSystem<Menu, Menu>(menuId.Value);
            if (!navbar.IsSuccess) return default;
            List<MenuItem> menus;
            var cache = await _cacheService.Get<List<MenuItem>>("_menus_central");
            if (cache == null || cache.Count == 0)
            {
                var search = await _service.GetAll<MenuItem, MenuItem>(x => x.MenuId.Equals(navbar.Result.Id));
                await _cacheService.Set("_menus_central", search.Result);
                menus = search.Result.ToList();
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
                Result = GetMenu(menus, roles)
            };
        }

        /// <summary>
        /// Get recursive menus
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="roles"></param>
        /// <param name="parentMenuId"></param>
        /// <returns></returns>
        private IEnumerable<MenuViewModel> GetMenu(IList<MenuItem> menus, IList<string> roles, Guid? parentMenuId = null)
        {
            var data = menus
                .Where(x => x.ParentMenuItemId == parentMenuId && HaveAccess(roles, x.AllowedRoles))
                .Adapt<IEnumerable<MenuViewModel>>().ToList();

            foreach (var t in data)
            {
                t.SubItems = GetMenu(menus, roles, t.Id).ToArray();
            }
            return data;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get menu roles
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetMenuRoles(Guid menuId)
        {
            var menu = await _service.FirstOrDefault<MenuItem>(x => x.Id == menuId);
            if (!menu.IsSuccess || menu.Result == null) return default;
            var preRoles = menu.Result.AllowedRoles;
            if (string.IsNullOrEmpty(preRoles)) return default;
            var roles = preRoles.Split("#").ToList();
            return roles;
        }

        /// <inheritdoc />
        /// <summary>
        /// Have user access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="menuItemAllowedRoles"></param>
        /// <returns></returns>
        public bool HaveAccess(IList<string> userRoles, string menuItemAllowedRoles)
        {
            if (string.IsNullOrEmpty(menuItemAllowedRoles)) return false;
            if (!userRoles.Any()) return false;
            try
            {
                var menuItemRoles = menuItemAllowedRoles.Split("#");
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
        public async Task<ResultModel<Guid>> UpdateMenuItemRoleAccess(Guid menuId, IList<string> roles)
        {
            var menu = await _service.FirstOrDefault<MenuItem>(x => x.Id == menuId);
            if (!menu.IsSuccess || menu.Result == null) return default;
            var model = menu.Result;
            if (!roles.Contains("Administrator")) roles.Add("Administrator");
            model.AllowedRoles = string.Join("#", roles.ToArray());
            await _cacheService.RemoveAsync("_menus_central");
            return await _service.UpdateSystem(model);
        }
    }
}
