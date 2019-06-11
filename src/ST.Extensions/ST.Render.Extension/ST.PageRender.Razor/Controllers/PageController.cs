using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Cache.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;
using ST.PageRender.Razor.ViewModels.PageViewModels;
using ST.Core;
using ST.Core.Attributes;
using ST.Core.BaseControllers;
using ST.Core.Helpers;
using ST.Forms.Abstractions;
using ST.Identity.Abstractions;
using ST.Identity.Data.MultiTenants;
using ST.PageRender.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;
using ST.PageRender.Razor.Helpers;
using ST.PageRender.Razor.Services.Abstractions;

namespace ST.PageRender.Razor.Controllers
{
    public class PageController : BaseController<ApplicationDbContext, EntitiesDbContext, ApplicationUser, ApplicationRole, Tenant, INotify<ApplicationRole>>
    {
        /// <summary>
        /// Inject  page render
        /// </summary>
        private readonly IPageRender _pageRender;
        private readonly IFormService _formService;
        private readonly IDynamicPagesContext _pagesContext;


        public PageController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify, IPageRender pageRender, IFormService formService, IDynamicPagesContext pagesContext) : base(userManager, roleManager, cacheService, applicationDbContext, context, notify)
        {
            _pageRender = pageRender;
            _formService = formService;
            _pagesContext = pagesContext;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get layouts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Layouts()
        {
            return View();
        }

        /// <summary>
        /// Get page scripts for manage
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PageScripts([Required] Guid pageId)
        {
            if (pageId == Guid.Empty) return NotFound();
            var scripts = await _pageRender.GetPageScripts(pageId);
            ViewBag.Scripts = scripts.Result.ToList();
            ViewBag.PageId = pageId;
            return View();
        }

        /// <summary>
        /// Get page  styles for manage
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PageStyles([Required] Guid pageId)
        {
            if (pageId == Guid.Empty) return NotFound();
            var styles = await _pageRender.GetPageStyles(pageId);
            ViewBag.Styles = styles.Result.ToList();
            ViewBag.PageId = pageId;
            return View();
        }

        /// <summary>
        /// Page Builder
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PageBuilder(Guid pageId)
        {
            if (pageId == Guid.Empty) return NotFound();
            var page = await _pageRender.GetPageAsync(pageId);
            if (page == null) return NotFound();
            ViewBag.Page = page;

            ViewBag.Css = page.Settings?.CssCode;
            ViewBag.Html = page.Settings?.HtmlCode;

            return View();
        }

        /// <summary>
        /// Save code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Save([Required]CodeUpdateViewModel model)
        {
            var req = await _pageRender.SavePageContent(model.PageId, model.HtmlCode, model.CssCode);
            if (!req.IsSuccess) return new JsonResult(req);
            var page = await _pageRender.GetPageAsync(model.PageId);
            await Notify.SendNotificationAsync(new SystemNotifications
            {
                Content = $"The page {page?.Settings?.Name} was updated with page builder!",
                Subject = "Info",
                NotificationTypeId = NotificationType.Info
            });
            return new JsonResult(req);
        }

        /// <summary>
        /// Get page code by type 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCode([Required]Guid id, [Required]string type)
        {
            if (Guid.Empty == id) return NotFound();
            var page = await _pageRender.GetPageAsync(id);
            if (page == null) return NotFound();
            if (string.IsNullOrEmpty(type)) return NotFound();
            var code = string.Empty;
            switch (type)
            {
                case "css":
                    code = page.Settings?.CssCode;
                    break;
                case "js":
                    code = page.Settings?.JsCode;
                    break;
                case "html":
                    code = page.Settings?.HtmlCode;
                    break;
            }
            var model = new CodeViewModel
            {
                PageId = page.Id,
                Code = code,
                Type = type
            };
            ViewBag.Page = page;
            return View(model);
        }

        /// <summary>
        /// Save code
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetCode(CodeViewModel model)
        {
            if (model.PageId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, "Invalid data input");
                return View(model);
            }

            var page = await _pageRender.GetPageAsync(model.PageId);
            if (page == null)
            {
                ModelState.AddModelError(string.Empty, "Page not found");
                return View(model);
            }

            if (page.Settings == null)
            {
                page.Settings = new PageSettings();
            }

            switch (model.Type)
            {
                case "css":
                    page.Settings.CssCode = model.Code;
                    break;
                case "js":
                    page.Settings.JsCode = model.Code;
                    break;
                case "html":
                    page.Settings.HtmlCode = model.Code;
                    break;
            }

            try
            {
                _pagesContext.Pages.Update(page);
                _pagesContext.SaveChanges();
                await CacheService.RemoveAsync($"_page_dynamic_{page.Id}");
                //return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
                return RedirectToAction("GetCode", new { type = model.Type, id = model.PageId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            ModelState.AddModelError(string.Empty, "Invalid data input");
            return View(model);
        }

        /// <summary>
        /// Create page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            var model = new PageViewModel
            {
                PageTypes = _pagesContext.PageTypes.ToList(),
                Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout)
            };

            return View(model);
        }

        /// <summary>
        /// Post Create page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(PageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PageTypes = _pagesContext.PageTypes.ToList();
                model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                return View(model);
            }

            var match = _pagesContext.Pages.Include(x => x.Settings)
                .FirstOrDefault(x => x.Settings.Name.ToLower().Equals(model.Name.ToLower()));

            if (match != null)
            {
                ModelState.AddModelError(string.Empty, "The page name exists, please type another name");
                model.PageTypes = _pagesContext.PageTypes.ToList();
                model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                return View(model);
            }

            if (!model.Path.StartsWith("/"))
            {
                ModelState.AddModelError(string.Empty, "Invalid format for path of page.Example: /PageName ");
                model.PageTypes = _pagesContext.PageTypes.ToList();
                model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                return View(model);
            }

            var page = new Page
            {
                Created = DateTime.Now,
                Changed = DateTime.Now,
                PageTypeId = model.PageTypeId,
                LayoutId = model.LayoutId,
                Path = model.Path,
                Settings = new PageSettings
                {
                    Name = model.Name,
                    Description = model.Description,
                    Title = model.Title
                },
                IsLayout = model.PageTypeId == PageManager.PageTypes[0].Id
            };

            try
            {
                _pagesContext.Pages.Add(page);
                _pagesContext.SaveChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                model.PageTypes = _pagesContext.PageTypes.ToList();
                model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                return View(model);
            }
            await Notify.SendNotificationAsync(new SystemNotifications
            {
                Content = $"New page added with name {page.Settings.Name}  and route {page.Path}",
                Subject = "Info",
                NotificationTypeId = NotificationType.Info
            });
            return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
        }

        /// <summary>
        /// Edit page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid pageId)
        {
            if (pageId == Guid.Empty) return NotFound();
            var page = await _pageRender.GetPageAsync(pageId);
            if (page == null) return NotFound();
            var model = page.Adapt<PageViewModel>();
            model.Name = page.Settings.Name;
            model.Description = page.Settings.Description;
            model.PageTypes = _pagesContext.PageTypes.AsNoTracking().ToList();
            model.Path = page.Path;
            model.Title = page.Settings.Title;
            model.Layouts = _pagesContext.Pages.AsNoTracking().Include(x => x.Settings).Where(x => x.IsLayout);
            return View(model);
        }

        /// <summary>
        /// Edit page post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(PageViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == Guid.Empty && model.SettingsId != Guid.Empty)
                {
                    model.PageTypes = _pagesContext.PageTypes.ToList();
                    model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                    ModelState.AddModelError(string.Empty, "Invalid page");
                    return View(model);
                }

                if (!model.Path.StartsWith("/"))
                {
                    ModelState.AddModelError(string.Empty, "Invalid format for path of page.Example: /PageName ");
                    model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                    model.PageTypes = _pagesContext.PageTypes.ToList();
                    return View(model);
                }

                var settings = _pagesContext.PageSettings.FirstOrDefault(x => x.Id.Equals(model.SettingsId));
                var page = _pagesContext.Pages.FirstOrDefault(x => x.Id.Equals(model.Id));
                if (page == null)
                {
                    model.PageTypes = _pagesContext.PageTypes.ToList();
                    model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                    ModelState.AddModelError(string.Empty, "Invalid page");
                    return View(model);
                }

                page.Changed = DateTime.Now;
                page.Path = model.Path;
                page.LayoutId = model.LayoutId;

                if (settings == null)
                {
                    model.PageTypes = _pagesContext.PageTypes.ToList();
                    model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                    ModelState.AddModelError(string.Empty, "Fail to get page settings");
                    return View(model);
                }
                settings.Description = model.Description;
                settings.Name = model.Name;
                settings.Changed = DateTime.Now;
                settings.Title = model.Title;

                try
                {
                    _pagesContext.PageSettings.Update(settings);
                    _pagesContext.Pages.Update(page);
                    await _pagesContext.SaveChangesAsync();
                    await CacheService.RemoveAsync($"_page_dynamic_{page.Id}");
                    return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
                }
                catch (Exception e)
                {
                    model.PageTypes = _pagesContext.PageTypes.ToList();
                    model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                    ModelState.AddModelError(string.Empty, e.Message);
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid data input");

            model.PageTypes = _pagesContext.PageTypes.ToList();
            return View(model);
        }


        /// <summary>
        /// Load pages with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadPages(DTParameters param)
        {
            var filtered = _pagesContext.FilterAbstractContext<Page>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount, x => !x.IsLayout && !x.IsDeleted).Select(x => new Page
                {
                    Id = x.Id,
                    Created = x.Created,
                    Changed = x.Changed,
                    Author = x.Author,
                    Settings = _pagesContext.PageSettings.FirstOrDefault(y => y.Id.Equals(x.SettingsId)),
                    PageType = x.PageType,
                    ModifiedBy = x.ModifiedBy,
                    IsDeleted = x.IsDeleted,
                    Layout = _pagesContext.Pages.Include(g => g.Settings).FirstOrDefault(y => y.Id == x.LayoutId),
                    SettingsId = x.SettingsId,
                    PageTypeId = x.PageTypeId,
                    IsSystem = x.IsSystem,
                    Path = x.Path
                }).ToList();

            var finalResult = new DTResult<Page>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count()
            };
            return Json(finalResult);
        }

        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadLayouts(DTParameters param)
        {
            var filtered = _pagesContext.FilterAbstractContext<Page>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount, x => x.IsLayout && !x.IsDeleted).Select(x => new Page
                {
                    Id = x.Id,
                    Created = x.Created,
                    Changed = x.Changed,
                    Author = x.Author,
                    Settings = _pagesContext.PageSettings.FirstOrDefault(y => y.Id.Equals(x.SettingsId)),
                    PageType = x.PageType,
                    ModifiedBy = x.ModifiedBy,
                    IsDeleted = x.IsDeleted,
                    SettingsId = x.SettingsId,
                    PageTypeId = x.PageTypeId,
                    IsSystem = x.IsSystem,
                    Path = x.Path
                }).ToList();

            var finalResult = new DTResult<Page>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count()
            };
            return Json(finalResult);
        }

        /// <summary>
        /// Delete page by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete page!", success = false });
            var page = _pagesContext.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
            if (page == null) return Json(new { message = "Fail to delete page!", success = false });
            if (page.IsSystem) return Json(new { message = "Fail to delete system page!", success = false });
            _pagesContext.Pages.Remove(page);

            try
            {
                _pagesContext.SaveChanges();
                CacheService.RemoveAsync($"_page_dynamic_{page.Id}");
                return Json(new { message = "Page was delete with success!", success = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Json(new { message = "Fail to delete form!", success = false });
        }

        /// <summary>
        /// Update scripts of page
        /// </summary>
        /// <param name="scripts"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateScripts([Required]IEnumerable<PageScript> scripts)
        {
            return await UpdateItemsAsync(scripts.ToList());
        }

        /// <summary>
        /// Update styles of page
        /// </summary>
        /// <param name="styles"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UpdateStyles([Required]IEnumerable<PageStyle> styles)
        {
            return await UpdateItemsAsync(styles.ToList());
        }

        /// <summary>
        /// Update scripts ans styles
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        [NonAction]
        private async Task<JsonResult> UpdateItemsAsync<TItem>(IList<TItem> items) where TItem : BaseModel, IPageItem
        {
            var result = new ResultModel();
            if (!items.Any())
            {
                result.IsSuccess = true;
                return Json(result);
            }

            var pageId = items.FirstOrDefault()?.PageId;
            var pageScripts = _pagesContext.SetEntity<TItem>().Where(x => x.PageId.Equals(pageId)).ToList();

            foreach (var prev in pageScripts)
            {
                var up = items.FirstOrDefault(x => x.Id.Equals(prev.Id));
                if (up == null)
                {
                    _pagesContext.SetEntity<TItem>().Remove(prev);
                }
                else if (prev.Order != up.Order || prev.Script != up.Script)
                {
                    prev.Script = up.Script;
                    prev.Order = up.Order;
                    _pagesContext.SetEntity<TItem>().Update(prev);
                }
            }

            var news = items.Where(x => x.Id == Guid.Empty).Select(x => new
            {
                PageId = pageId,
                x.Script,
                x.Order
            }).Adapt<IEnumerable<TItem>>().ToList();

            if (news.Any())
            {
                _pagesContext.SetEntity<TItem>().AddRange(news);
            }

            try
            {
                _pagesContext.SaveChanges();
                await CacheService.RemoveAsync($"_page_dynamic_{pageId}");
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Generate page
        /// </summary>
        /// <param name="name"></param>
        /// <param name="viewModelId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GeneratePage([Required] string name, [Required] Guid viewModelId)
        {
            return Json(await _pageRender.GenerateListPageType(name, $"{name}-page", viewModelId));
        }

        /// <summary>
        /// Scaffold pages 
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Scaffold(Guid? tableId)
        {
            if (tableId == null) return NotFound();

            var table = await Context.Table.FirstOrDefaultAsync(x => x.Id == tableId);
            if (table == null) return NotFound();
            var viewModel = await _pageRender.GenerateViewModel(tableId.Value);
            var listPath = $"{table.Name}-{Guid.NewGuid()}-page";
            if (!viewModel.IsSuccess) return NotFound();
            var createForm = await _formService.GenerateFormByEntity(table.Id, $"Add {table.Name} {Guid.NewGuid()}", $"/{listPath}", $"Add {table.Name}");
            if (createForm != null)
            {
                var resCreate = _formService.CreateForm(createForm);
                if (resCreate.IsSuccess)
                {
                    await _pageRender.GenerateFormPage(resCreate.Result, $"/{listPath}/add", $"Add {table.Name}");
                }
            }

            var editForm = await _formService.GenerateFormByEntity(table.Id, $"Edit {table.Name} {Guid.NewGuid()}", $"/{listPath}", $"Edit {table.Name}");
            if (editForm != null)
            {
                var resEdit = _formService.CreateForm(editForm);
                if (resEdit.IsSuccess)
                {
                    await _pageRender.GenerateFormPage(resEdit.Result, $"/{listPath}/edit", $"Edit {table.Name}");
                }
            }

            var listPage = await _pageRender.GenerateListPageType(table.Name, listPath, viewModel.Result, $"/{listPath}/add", $"/{listPath}/edit");
            if (listPage == null) return NotFound();
            if (listPage.IsSuccess)
            {
                return RedirectToAction("Edit", "Table", new
                {
                    table.Id
                });
            }
            return NotFound();
        }

        /// <summary>
        /// Manage page acl
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PageAcl([Required] Guid pageId)
        {
            var page = await _pagesContext.Pages
                .Include(x => x.RolePagesAcls)
                .FirstOrDefaultAsync(x => x.Id == pageId);
            if (page == null) return NotFound();
            return View();
        }
    }
}