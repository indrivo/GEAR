using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.PageRender.Abstractions;
using GR.UI.Menu.Abstractions;
using GR.UI.Menu.Abstractions.Helpers;
using GR.UI.Menu.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GR.UI.Menu.Razor.Controllers
{
    [Authorize]
    [GearAuthorize(GlobalResources.Roles.ADMINISTRATOR)]
    public class MenuController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject dynamic page context
        /// </summary>
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Inject menu service
        /// </summary>
        private readonly IMenuService _menuService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject menu context
        /// </summary>
        private readonly IMenuDbContext _context;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly IMemoryCache _cacheService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pagesContext"></param>
        /// <param name="menuService"></param>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        /// <param name="cacheService"></param>
        public MenuController(IDynamicPagesContext pagesContext, IMenuService menuService, IUserManager<GearUser> userManager, IMenuDbContext context, IMemoryCache cacheService)
        {
            _pagesContext = pagesContext;
            _menuService = menuService;
            _userManager = userManager;
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new page type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(MenuGroup model)
        {
            if (model != null)
            {
                var req = await _menuService.CreateMenuGroupAsync(model);
                if (req.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Fail to save!");
            }

            return View(model);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var model = await _menuService.FindMenuGroupByIdAsync(id);
            if (!model.IsSuccess) return NotFound();
            return View(model.Result);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(MenuGroup model)
        {
            if (model == null) return NotFound();
            var req = await _menuService.UpdateMenuGroupAsync(model);
            if (req.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(req.Errors);
            return View(model);
        }

        /// <summary>
        /// Get menu items
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetMenu(Guid menuId, Guid? parentId = null)
        {
            ViewBag.MenuId = menuId;
            ViewBag.ParentId = parentId;
            ViewBag.Menu = (await _menuService.FindMenuGroupByIdAsync(menuId)).Result;
            ViewBag.Parent = (await _menuService.FindMenuItemByIdAsync(parentId)).Result;
            return View();
        }

        /// <summary>
        /// Create menu item
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateItem(Guid menuId, Guid? parentId = null)
        {
            ViewBag.MenuId = menuId;
            ViewBag.ParentId = parentId;
            ViewBag.Routes = _pagesContext.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path)
                .OrderBy(x => x);
            return View(new MenuItem
            {
                MenuId = menuId,
                ParentMenuItemId = parentId
            });
        }

        /// <summary>
        /// Create menu item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateItem(MenuItem model)
        {
            ViewBag.Routes = _pagesContext.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
            if (!ModelState.IsValid) return View(model);
            var req = await _menuService.CreateMenuItemAsync(model);
            if (req.IsSuccess)
            {
                return RedirectToAction(nameof(GetMenu), new
                {
                    model.MenuId,
                    ParentId = model.ParentMenuItemId
                });
            }

            ModelState.AppendResultModelErrors(req.Errors);

            return View(model);
        }

        /// <summary>
        /// Edit item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditItem(Guid itemId)
        {
            ViewBag.Routes = _pagesContext.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path)
                .OrderBy(x => x);
            var item = await _menuService.FindMenuItemByIdAsync(itemId);
            if (!item.IsSuccess) return NotFound();
            return View(item.Result);
        }

        /// <summary>
        /// Edit item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditItem(MenuItem model)
        {
            var rq = await _menuService.UpdateMenuItemAsync(model);
            if (rq.IsSuccess)
                return RedirectToAction("GetMenu", new
                {
                    model.MenuId,
                    ParentId = model.ParentMenuItemId
                });

            ViewBag.Routes = _pagesContext.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
            ModelState.AddModelError(string.Empty, "Fail to save!");
            return View(model);
        }

        /// <summary>
        /// Load menu items
        /// </summary>
        /// <param name="param"></param>
        /// <param name="menuId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadMenuItems(DTParameters param, Guid menuId, Guid? parentId = null)
            => Json(_menuService.GetPaginatedMenuItems(param, menuId, parentId));

        /// <summary>
        /// Load page types with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadMenuGroups(DTParameters param)
            => Json(_menuService.GetPaginatedMenuGroups(param));

        /// <summary>
        /// Delete page type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Delete(Guid id)
        {
            var removeRequest = await _context.RemoveByIdAsync<MenuGroup, Guid>(id);
            if (!removeRequest.IsSuccess) return Json(new { message = "Fail to delete menu!", success = false });
            _cacheService.Remove(MenuHelper.GetCacheKey(id.ToString()));
            return Json(new { message = "Menu was delete with success!", success = true });
        }

        /// <summary>
        /// Delete page type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> DeleteMenuItem(Guid id)
        {
            var find = await _context.FindByIdAsync<MenuItem, Guid>(id);
            var dbOperation = await _context.RemoveByIdAsync<MenuItem, Guid>(id);
            if (!dbOperation.IsSuccess) return Json(new { message = "Fail to delete menu item!", success = false });
            _cacheService.Remove(MenuHelper.GetCacheKey(find.Result.MenuId.ToString()));
            return Json(new { message = "Model was delete with success!", success = true });
        }


        /// <summary>
        /// Get menu item roles
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<JsonResult> GetMenuItemRoles([Required] Guid menuId)
        {
            if (menuId == Guid.Empty) return Json(new ResultModel());
            var roles = await _menuService.GetMenuRoles(menuId);

            return Json(roles);
        }

        /// <summary>
        /// Get menus
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<JsonResult> GetMenus(Guid? menuId = null)
        {
            if (menuId == null)
            {
                menuId = MenuResources.AppMenuId;
            }
            IList<string> roles = new List<string>();
            var user = await _userManager.UserManager.GetUserAsync(User);
            if (user != null)
            {
                roles = await _userManager.UserManager.GetRolesAsync(user);
            }
            var req = await _menuService.GetMenus(menuId, roles);
            return Json(req);
        }

        /// <summary>
        /// Update roles
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<JsonResult> UpdateMenuItemRoleAccess([Required] Guid menuId, IList<string> roles)
        {
            return Json(await _menuService.UpdateMenuItemRoleAccess(menuId, roles));
        }

        /// <summary>
        /// Get view for manage menu as a tree
        /// </summary>
        /// <param name="menuBlockId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OrderMenuItemsAsTree([Required]Guid menuBlockId)
        {
            ViewBag.menuBlockId = menuBlockId;
            return View();
        }

        /// <summary>
        /// Get menu tree
        /// </summary>
        /// <param name="menuBlockId"></param>
        /// <returns></returns>
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        [HttpPost]
        public async Task<JsonResult> GetMenuTreeByMenuBlockId([Required]Guid menuBlockId)
        {
            var tree = await _menuService.GetMenus(menuBlockId, new List<string> { GlobalResources.Roles.ADMINISTRATOR });
            return Json(tree);
        }

        /// <summary>
        /// Get page scripts for manage
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OrderMenuChildItems([Required] Guid? parentId)
        {
            var items = await _menuService.GetChildsOfMenuItemAsync(parentId);
            if (!items.IsSuccess) return NotFound();
            ViewBag.Items = items.Result.ToList();
            ViewBag.ParentId = parentId;
            return View();
        }
    }
}