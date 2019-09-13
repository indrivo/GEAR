using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.Cache.Abstractions;
using ST.DynamicEntityStorage.Abstractions;
using ST.Core;
using ST.Core.Attributes;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions.Enums;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.PageRender.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;
using ST.PageRender.Razor.Services;

namespace ST.PageRender.Razor.Controllers
{
    public class MenuController : Controller
    {

        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
		/// Inject Data Service
		/// </summary>
		private readonly IDynamicService _service;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Inject menu service
        /// </summary>
        private readonly IMenuService _menuService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="cacheService"></param>
        /// <param name="pagesContext"></param>
        /// <param name="menuService"></param>
        public MenuController(IDynamicService service, ICacheService cacheService, IDynamicPagesContext pagesContext, IMenuService menuService)
        {
            _service = service;
            _cacheService = cacheService;
            _pagesContext = pagesContext;
            _menuService = menuService;
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
        public async Task<IActionResult> Create(Menu model)
        {
            if (model != null)
            {
                var req = await _service.AddWithReflection(model);
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
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id.Equals(Guid.Empty)) return NotFound();
            var model = await _service.GetByIdWithReflection<Menu, Menu>(id);

            if (!model.IsSuccess) return NotFound();

            return View(model.Result);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Menu model)
        {
            if (model == null) return NotFound();
            var dataModel = (await _service.GetByIdWithReflection<Menu, Menu>(model.Id)).Result;

            if (dataModel == null) return NotFound();

            dataModel.Name = model.Name;
            dataModel.Description = model.Description;
            dataModel.Author = model.Author;
            dataModel.Changed = DateTime.Now;
            var req = await _service.UpdateWithReflection(dataModel);
            if (req.IsSuccess) return RedirectToAction("Index");
            ModelState.AddModelError(string.Empty, "Fail to save");
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
            ViewBag.Menu = (await _service.GetByIdWithReflection<Menu, Menu>(menuId)).Result;
            ViewBag.Parent = (parentId != null) ?
                                    (await _service.GetByIdWithReflection<MenuItem, MenuItem>(parentId.Value)).Result
                                    : null;
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
            return View();
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
            if (model != null)
            {
                model.AllowedRoles = "Administrator#";
                var data = await _service.GetAllWhitOutInclude<MenuItem, MenuItem>(x =>
                    x.ParentMenuItemId == model.ParentMenuItemId);
                if (data.IsSuccess)
                {
                    model.Order = data.Result?.Max(x => x.Order) + 1 ?? 1;
                }
                var req = await _service.AddWithReflection(model);
                if (req.IsSuccess)
                {
                    await _cacheService.RemoveAsync(MenuHelper.GetCacheKey(model.MenuId.ToString()));
                    return RedirectToAction("GetMenu", new
                    {
                        model.MenuId,
                        ParentId = model.ParentMenuItemId
                    });
                }

                ModelState.AddModelError(string.Empty, "Fail to save!");
            }

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
            var item = await _service.GetByIdWithReflection<MenuItem, MenuItem>(itemId);
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
            var rq = await _service.UpdateWithReflection(model);
            if (rq.IsSuccess)
            {
                await _cacheService.RemoveAsync(MenuHelper.GetCacheKey(model.MenuId.ToString()));
                return RedirectToAction("GetMenu", new
                {
                    model.MenuId,
                    ParentId = model.ParentMenuItemId
                });
            }

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
        public async Task<JsonResult> LoadMenuItems(DTParameters param, Guid menuId, Guid? parentId = null)
        {
            var (data, count) = await _service.Filter<MenuItem>(param.Search.Value, param.SortOrder, param.Start,
                param.Length, x => x.MenuId.Equals(menuId) && x.ParentMenuItemId.Equals(parentId));

            var finalResult = new DTResult<MenuItem>
            {
                Draw = param.Draw,
                Data = data.OrderBy(x => x.Order).ToList(),
                RecordsFiltered = count,
                RecordsTotal = data.Count()
            };
            return Json(finalResult);
        }

        /// <summary>
        /// Load page types with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public async Task<JsonResult> LoadPages(DTParameters param)
        {
            var filtered = await _service.Filter<Menu>(param.Search.Value, param.SortOrder, param.Start,
                param.Length);

            var finalResult = new DTResult<Menu>
            {
                Draw = param.Draw,
                Data = filtered.Item1,
                RecordsFiltered = filtered.Item2,
                RecordsTotal = filtered.Item1.Count()
            };
            return Json(finalResult);
        }

        /// <summary>
        /// Delete page type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete menu!", success = false });
            var menu = await _service.DeletePermanent<Menu>(Guid.Parse(id));
            if (!menu.IsSuccess) return Json(new { message = "Fail to delete menu!", success = false });
            await _cacheService.RemoveAsync(MenuHelper.GetCacheKey(id));
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
        public async Task<JsonResult> DeleteMenuItem(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete menu item!", success = false });
            var menu = await _service.GetByIdWithReflection<MenuItem, MenuItem>(id.ToGuid());
            if (!menu.IsSuccess) return Json(new { message = "Fail to delete menu item!", success = false });
            var dbOperation = await _service.DeletePermanent<MenuItem>(id.ToGuid());
            if (!dbOperation.IsSuccess) return Json(new { message = "Fail to delete menu item!", success = false });
            await _cacheService.RemoveAsync(MenuHelper.GetCacheKey(menu.Result.MenuId.ToString()));
            return Json(new { message = "Model was delete with success!", success = true });
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
            var items = await _service.GetAllWhitOutInclude<MenuItem, MenuItem>(filters: new List<Filter>
            {
                new Filter{Value = parentId, Criteria = Criteria.Equals, Parameter = nameof(MenuItem.ParentMenuItemId)}
            });
            if (!items.IsSuccess) return NotFound();
            ViewBag.Items = items.Result.ToList();
            ViewBag.ParentId = parentId;
            return View();
        }
    }
}