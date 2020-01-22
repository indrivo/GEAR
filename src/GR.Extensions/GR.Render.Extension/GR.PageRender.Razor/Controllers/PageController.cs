using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.BaseControllers;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Data;
using GR.Forms.Abstractions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Data;
using GR.Notifications.Abstractions;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Events;
using GR.PageRender.Abstractions.Events.EventArgs;
using GR.PageRender.Abstractions.Helpers;
using GR.PageRender.Abstractions.Models.Pages;
using GR.PageRender.Abstractions.Models.PagesACL;
using GR.PageRender.Razor.ViewModels.PageViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GR.PageRender.Razor.Controllers
{
    public class PageController : BaseIdentityController<ApplicationDbContext, EntitiesDbContext, GearUser, GearRole, Tenant, INotify<GearRole>>
    {
        #region Injectable

        /// <summary>
        /// Inject  page render
        /// </summary>
        private readonly IPageRender _pageRender;

        private readonly IFormService _formService;
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Inject memory cache
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        #endregion Injectable

        public PageController(UserManager<GearUser> userManager, RoleManager<GearRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<GearRole> notify, IPageRender pageRender, IFormService formService, IDynamicPagesContext pagesContext, IMemoryCache memoryCache) : base(userManager, roleManager, applicationDbContext, context, notify)
        {
            _cacheService = cacheService;
            _pageRender = pageRender;
            _formService = formService;
            _pagesContext = pagesContext;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index() => View();

        /// <summary>
        /// Get layouts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Layouts() => View();

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
            var saveRequest = await _pageRender.SavePageContent(model.PageId, model.HtmlCode, model.CssCode);
            return Json(saveRequest);
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
                await _cacheService.RemoveAsync($"_page_dynamic_{page.Id}");
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

            var match = await _pagesContext.Pages.Include(x => x.Settings)
                .FirstOrDefaultAsync(x => x.Settings.Name.ToLower().Equals(model.Name.ToLower()));

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
                    Title = model.Title,
                    TitleTranslateKey = model.TitleTranslateKey
                },
                IsLayout = model.PageTypeId == PageSeeder.PageTypes[0].Id
            };

            await _pagesContext.Pages.AddAsync(page);
            var dbResult = await _pagesContext.PushAsync();
            if (dbResult.IsSuccess)
            {
                DynamicUiEvents.Pages.PageCreated(new PageCreatedEventArgs
                {
                    PageId = page.Id,
                    PageName = page.Settings.Name
                });

                return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
            }

            ModelState.AppendResultModelErrors(dbResult.Errors);
            model.PageTypes = _pagesContext.PageTypes.ToList();
            model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
            return View(model);
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
            model.TitleTranslateKey = page.Settings.TitleTranslateKey;
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
                settings.TitleTranslateKey = model.TitleTranslateKey;

                var dbResult = await _pagesContext.PushAsync();
                if (dbResult.IsSuccess)
                {
                    _pagesContext.PageSettings.Update(settings);
                    _pagesContext.Pages.Update(page);
                    await _pagesContext.SaveChangesAsync();
                    RemovePageFromCache(page.Id);
                    DynamicUiEvents.Pages.PageUpdated(new PageCreatedEventArgs
                    {
                        PageId = page.Id,
                        PageName = page.Settings?.Name
                    });

                    return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
                }
                model.PageTypes = _pagesContext.PageTypes.ToList();
                model.Layouts = _pagesContext.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
                ModelState.AppendResultModelErrors(dbResult.Errors);
                return View(model);
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
        public async Task<JsonResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete page!", success = false });
            var page = _pagesContext.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
            if (page == null) return Json(new { message = "Fail to delete page!", success = false });
            if (page.IsSystem) return Json(new { message = "Fail to delete system page!", success = false });
            _pagesContext.Pages.Remove(page);

            var dbResult = await _pagesContext.PushAsync();
            if (!dbResult.IsSuccess) return Json(new { message = "Fail to delete form!", success = false });
            DynamicUiEvents.Pages.PageDeleted(new PageCreatedEventArgs
            {
                PageId = page.Id,
                PageName = page.Settings?.Name
            });
            RemovePageFromCache(page.Id);
            return Json(new { message = "Page was delete with success!", success = true });
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
                await _pagesContext.SaveChangesAsync();
                RemovePageFromCache(pageId.GetValueOrDefault(Guid.Empty));
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

            var key = $"entity_Scaffold";

            var tables = _memoryCache.Get<IEnumerable<TableModel>>(key)?.ToList() ?? new List<TableModel>();
            var table = tables.FirstOrDefault(x => x.Id == tableId) ?? await Context.Table.FirstOrDefaultAsync(x => x.Id == tableId);

            if (tables.FirstOrDefault(x => x.Id == tableId) == null)
            {
                tables.Add(table);
                _memoryCache.Set(key, tables);
            }

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
                // ReSharper disable once Mvc.ActionNotResolved
                // ReSharper disable once Mvc.ControllerNotResolved
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
                .Include(x => x.Settings)
                .Include(x => x.RolePagesAcls)
                .FirstOrDefaultAsync(x => x.Id == pageId);
            if (page == null) return NotFound();
            var roles = RoleManager.Roles.Where(x => !x.IsDeleted).ToList();
            ViewBag.Roles = roles;
            var rolesAcl = roles.ToDictionary(x => x, x => page.RolePagesAcls.FirstOrDefault(y => y.RoleId == Guid.Parse(x.Id)));
            ViewBag.ACL = rolesAcl;
            return View(page);
        }

        /// <summary>
        /// Enable/Disable ACL
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="enableAcl"></param>
        /// <returns></returns>
        public async Task<JsonResult> ChangeAclEnableStateAsync([Required]Guid pageId, bool enableAcl)
        {
            var rs = new ResultModel();
            var page = await _pagesContext.Pages
                .Include(x => x.RolePagesAcls)
                .FirstOrDefaultAsync(x => x.Id == pageId);
            if (page == null)
            {
                rs.Errors.Add(new ErrorModel(string.Empty, "Page not found!"));
                return Json(rs);
            }

            page.IsEnabledAcl = enableAcl;
            try
            {
                _pagesContext.Update(page);
                if (!enableAcl)
                {
                    _pagesContext.RolePagesAcls.RemoveRange(page.RolePagesAcls);
                }
                await _pagesContext.SaveChangesAsync();
                rs.IsSuccess = true;
                RemovePageFromCache(pageId);
            }
            catch (Exception e)
            {
                rs.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }
            return Json(rs);
        }

        /// <summary>
        /// Remove page from cache
        /// </summary>
        /// <param name="pageId"></param>
        [NonAction]
        private async void RemovePageFromCache(Guid pageId)
        {
            await _cacheService.RemoveAsync($"_page_dynamic_{pageId}");
        }

        /// <summary>
        /// Change access to page by role id and page id
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pageId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ChangeAccessToPageByRole([Required]Guid roleId, [Required]Guid pageId, bool state)
        {
            var rs = new ResultModel();
            var page = await _pagesContext.Pages
                .Include(x => x.RolePagesAcls)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pageId);
            if (page == null)
            {
                rs.Errors.Add(new ErrorModel(string.Empty, "Page not found!"));
                return Json(rs);
            }

            var entry = page.RolePagesAcls.FirstOrDefault(x => x.RoleId == roleId);
            if (entry == null)
            {
                _pagesContext.RolePagesAcls.Add(new RolePagesAcl
                {
                    RoleId = roleId,
                    AllowAccess = state,
                    PageId = page.Id
                });
            }
            else
            {
                var model = new RolePagesAcl
                {
                    AllowAccess = state,
                    RoleId = roleId,
                    PageId = pageId
                };
                _pagesContext.RolePagesAcls.Remove(entry);
                _pagesContext.RolePagesAcls.Add(model);
            }

            try
            {
                await _pagesContext.SaveChangesAsync();
                rs.IsSuccess = true;
                RemovePageFromCache(pageId);
            }
            catch (Exception e)
            {
                rs.Errors.Add(new ErrorModel(string.Empty, e.Message));
            }
            return Json(rs);
        }
    }
}