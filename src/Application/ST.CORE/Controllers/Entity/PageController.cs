using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.BaseBusinessRepository;
using ST.BaseRepository;
using ST.CORE.Attributes;
using ST.CORE.Installation;
using ST.CORE.Models;
using ST.CORE.Services.Abstraction;
using ST.CORE.ViewModels.Pages;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Notifications;
using ST.Entities.Models.Pages;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Services.Abstractions;
using ST.Notifications.Abstraction;
using ST.Procesess.Data;

namespace ST.CORE.Controllers.Entity
{
	public class PageController : BaseController
	{
		/// <summary>
		/// Inject  page render
		/// </summary>
		private readonly IPageRender _pageRender;
		private readonly IHostingEnvironment _env;

		public PageController(EntitiesDbContext context, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, INotify notify, IOrganizationService organizationService, ProcessesDbContext processesDbContext, IPageRender pageRender, IHostingEnvironment env) : base(context, applicationDbContext, userManager, roleManager, notify, organizationService, processesDbContext)
		{
			_pageRender = pageRender;
			_env = env;
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
		public IActionResult PageScripts([Required] Guid pageId)
		{
			if (pageId == Guid.Empty) return NotFound();
			var scripts = _pageRender.GetPageScripts(pageId);
			ViewBag.Scripts = scripts.Result.OrderBy(x => x.Order).ToList();
			return View();
		}

		/// <summary>
		/// Get page  styles for manage
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult PageStyles([Required] Guid pageId)
		{
			if (pageId == Guid.Empty) return NotFound();
			var styles = _pageRender.GetPageStyles(pageId);
			ViewBag.Styles = styles.Result.OrderBy(x => x.Order).ToList();
			return View();
		}

		/// <summary>
		/// Page Builder
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult PageBuilder(Guid pageId)
		{
			if (pageId == Guid.Empty) return NotFound();
			var page = Context.Pages.FirstOrDefault(x => x.Id.Equals(pageId));
			if (page == null) return NotFound();
			page.Settings = Context.PageSettings.FirstOrDefault(x => x.Id.Equals(page.SettingsId));
			ViewBag.Page = page;

			ViewBag.Css = _pageRender.GetPageCss(page.Settings?.Name);
			ViewBag.Html = _pageRender.GetPageHtml(page.Settings?.Name);

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
			var req = _pageRender.SavePageContent(model.PageId, model.HtmlCode, model.CssCode);
			if (req.IsSuccess)
			{
				var page = Context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(model.PageId));
				await Notify.SendNotificationAsync(new SystemNotifications
				{
					Content = $"The page {page?.Settings?.Name} was updated with page builder!",
					Subject = "Info",
					NotificationTypeId = NotificationType.Info
				});
			}
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
			var page = await Context.Pages.Where(x => x.Id == id).Include(x => x.Settings).FirstOrDefaultAsync();
			if (page == null) return NotFound();
			if (string.IsNullOrEmpty(type)) return NotFound();
			var file = page.Settings.PhysicPath.Split("\\").LastOrDefault();
			var path = Path.Combine(page.Settings.PhysicPath, $"{file}.{type}");
			var exists = System.IO.File.Exists(path);
			if (!exists) return NotFound();
			var code = System.IO.File.ReadAllText(path);
			var model = new CodeViewModel
			{
				PageId = page.Id,
				Path = path,
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
		public IActionResult GetCode(CodeViewModel model)
		{
			if (model.PageId == Guid.Empty)
			{
				ModelState.AddModelError(string.Empty, "Invalid data input");
				return View(model);
			}

			if (!string.IsNullOrEmpty(model.Path))
			{
				System.IO.File.WriteAllText(model.Path, model.Code);
				var page = Context.Pages.FirstOrDefault(x => x.Id.Equals(model.PageId));
				return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid data input");
				return View(model);
			}
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
				PageTypes = Context.PageTypes.ToList(),
				Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout)
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
				model.PageTypes = Context.PageTypes.ToList();
				model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
				return View(model);
			}

			var match = Context.Pages.Include(x => x.Settings)
				.FirstOrDefault(x => x.Settings.Name.ToLower().Equals(model.Name.ToLower()));

			if (match != null)
			{
				ModelState.AddModelError(string.Empty, "The page name exists, please type another name");
				model.PageTypes = Context.PageTypes.ToList();
				model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
				return View(model);
			}

			if (!model.Path.StartsWith("/"))
			{
				ModelState.AddModelError(string.Empty, "Invalid format for path of page.Example: /PageName ");
				model.PageTypes = Context.PageTypes.ToList();
				model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
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
				IsLayout = model.PageTypeId == WebPageSync.PageTypes[0].Id
			};

			try
			{
				var req = _pageRender.CreatePage(model.Name.ToLower());
				if (!req.IsSuccess)
				{
					ModelState.AddModelError(string.Empty, "Fail to create files");
					model.PageTypes = Context.PageTypes.ToList();
					model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
					return View(model);
				}

				page.Settings.PhysicPath = req.Result;

				Context.Pages.Add(page);
				Context.SaveChanges();
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				model.PageTypes = Context.PageTypes.ToList();
				model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
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
		public IActionResult Edit(Guid pageId)
		{
			if (pageId == Guid.Empty) return NotFound();
			var page = Context.Pages.AsNoTracking().Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(pageId));
			if (page == null) return NotFound();
			var model = page.Adapt<PageViewModel>();
			model.Name = page.Settings.Name;
			model.Description = page.Settings.Description;
			model.PageTypes = Context.PageTypes.AsNoTracking().ToList();
			model.Path = page.Path;
			model.Title = page.Settings.Title;
			model.Layouts = Context.Pages.AsNoTracking().Include(x => x.Settings).Where(x => x.IsLayout);
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
					model.PageTypes = Context.PageTypes.ToList();
					model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
					ModelState.AddModelError(string.Empty, "Invalid page");
					return View(model);
				}

				if (!model.Path.StartsWith("/"))
				{
					ModelState.AddModelError(string.Empty, "Invalid format for path of page.Example: /PageName ");
					model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
					model.PageTypes = Context.PageTypes.ToList();
					return View(model);
				}



				var settings = Context.PageSettings.FirstOrDefault(x => x.Id.Equals(model.SettingsId));
				var page = Context.Pages.FirstOrDefault(x => x.Id.Equals(model.Id));
				if (page == null)
				{
					model.PageTypes = Context.PageTypes.ToList();
					model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
					ModelState.AddModelError(string.Empty, "Invalid page");
					return View(model);
				}

				page.Changed = DateTime.Now;
				page.Path = model.Path;
				page.LayoutId = model.LayoutId;

				if (settings == null)
				{
					model.PageTypes = Context.PageTypes.ToList();
					model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
					ModelState.AddModelError(string.Empty, "Fail to get page settings");
					return View(model);
				}
				_pageRender.UpdatePageName(settings.PhysicPath, settings.Name.ToLower(), model.Name);
				settings.Description = model.Description;
				settings.Name = model.Name;
				settings.Changed = DateTime.Now;
				settings.Title = model.Title;

				try
				{
					Context.PageSettings.Update(settings);
					Context.Pages.Update(page);
					await Context.SaveChangesAsync();

					return RedirectToAction(page.IsLayout ? "Layouts" : "Index");
				}
				catch (Exception e)
				{
					model.PageTypes = Context.PageTypes.ToList();
					model.Layouts = Context.Pages.Include(x => x.Settings).Where(x => x.IsLayout);
					ModelState.AddModelError(string.Empty, e.Message);
					return View(model);
				}
			}

			ModelState.AddModelError(string.Empty, "Invalid data input");

			model.PageTypes = Context.PageTypes.ToList();
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
			var filtered = Context.Filter<Page>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount, x => !x.IsLayout && !x.IsDeleted).Select(x => new Page
				{
					Id = x.Id,
					Created = x.Created,
					Changed = x.Changed,
					Author = x.Author,
					Settings = Context.PageSettings.FirstOrDefault(y => y.Id.Equals(x.SettingsId)),
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
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count()
			};
			return Json(finalResult);
		}

		[HttpPost]
		[AjaxOnly]
		public JsonResult LoadLayouts(DTParameters param)
		{
			var filtered = Context.Filter<Page>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount, x => x.IsLayout && !x.IsDeleted).Select(x => new Page
				{
					Id = x.Id,
					Created = x.Created,
					Changed = x.Changed,
					Author = x.Author,
					Settings = Context.PageSettings.FirstOrDefault(y => y.Id.Equals(x.SettingsId)),
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
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count()
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
			var page = Context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (page == null) return Json(new { message = "Fail to delete page!", success = false });
			if (page.IsSystem) return Json(new { message = "Fail to delete system page!", success = false });
			Context.Pages.Remove(page);

			try
			{
				Context.SaveChanges();
				_pageRender.DeletePage(page.Settings.PhysicPath);
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
		public JsonResult UpdateScripts([Required]IEnumerable<PageScript> scripts)
		{
			return UpdateItems(scripts.ToList());
		}

		/// <summary>
		/// Update styles of page
		/// </summary>
		/// <param name="styles"></param>
		/// <returns></returns>
		[HttpPost]
		public JsonResult UpdateStyles([Required]IEnumerable<PageStyle> styles)
		{
			return UpdateItems(styles.ToList());
		}

		/// <summary>
		/// Update scripts ans styles
		/// </summary>
		/// <typeparam name="TItem"></typeparam>
		/// <param name="items"></param>
		/// <returns></returns>
		[NonAction]
		private JsonResult UpdateItems<TItem>(IList<TItem> items) where TItem : BaseModel, IPageItem
		{
			var result = new ResultModel();
			if (!items.Any())
			{
				result.IsSuccess = true;
				return Json(result);
			}

			var pageId = items.First().PageId;
			var pageScripts = Context.Set<TItem>().Where(x => x.PageId.Equals(pageId)).ToList();

			foreach (var prev in pageScripts)
			{
				var up = items.FirstOrDefault(x => x.Id.Equals(prev.Id));
				if (up == null)
				{
					Context.Set<TItem>().Remove(prev);
				}
				else if (prev.Order != up.Order || prev.Script != up.Script)
				{
					prev.Script = up.Script;
					prev.Order = up.Order;
					Context.Set<TItem>().Update(prev);
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
				Context.Set<TItem>().AddRange(news);
			}

			try
			{
				Context.SaveChanges();
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
			if (string.IsNullOrEmpty(name) || viewModelId.Equals(Guid.Empty)) return Json(default(ResultModel));
			var match = Context.Pages.Include(x => x.Settings)
				.FirstOrDefault(x => x.Settings.Name.ToLower().Equals(name.ToLower()));
			var viewModel = Context.ViewModels
					.Include(x => x.TableModel)
					.Include(x => x.ViewModelFields)
					.FirstOrDefault(x => x.Id.Equals(viewModelId));
			if (viewModel == null) return Json(default(ResultModel));

			if (match != null)
			{
				return Json(default(ResultModel));
			}

			var pageId = Guid.NewGuid();

			var page = new Page
			{
				Id = pageId,
				Created = DateTime.Now,
				Changed = DateTime.Now,
				PageTypeId = WebPageSync.PageTypes[1].Id,
				LayoutId = WebPageSync.Layouts[0],
				Path = $"/{name}",
				Settings = new PageSettings
				{
					Name = name,
					Description = "Generated page",
					Title = name
				},
				IsLayout = false
			};

			try
			{
				var req = _pageRender.CreatePage(name);

				if (!req.IsSuccess)
				{
					return Json(default(ResultModel));
				}


				page.Settings.PhysicPath = req.Result;

				Context.Pages.Add(page);
				Context.SaveChanges();

				var fileInfo = _env.ContentRootFileProvider.GetFileInfo($"{BasePath}/listDefaultTemplate.html");
				var jsTemplateFileInfo = _env.ContentRootFileProvider.GetFileInfo($"{BasePath}/listDefaultTemplate.js");
				var reader = new StreamReader(fileInfo.CreateReadStream());
				var readerJs = new StreamReader(jsTemplateFileInfo.CreateReadStream());
				var template = await reader.ReadToEndAsync();
				var listId = Guid.NewGuid();
				template = template.Replace("#Title", name);
				template = template.Replace("#SubTitle", name);
				template = template.Replace("#EntityName", viewModel.TableModel.Name);
				template = template.Replace("#EntityId", viewModel.TableModelId.ToString());
				template = template.Replace("#ViewModelId", viewModel.Id.ToString());
				template = template.Replace("#ListId", listId.ToString());

				var tableHead = new StringBuilder();

				foreach (var line in viewModel.ViewModelFields.ToList().OrderBy(x => x.Order))
					tableHead.AppendLine($"<th translate='{line.Translate}'>{line.Name}</th>");
				tableHead.AppendLine("<th>Actions</th>");

				template = template.Replace("#TableHead", tableHead.ToString());

				var templateJs = await readerJs.ReadToEndAsync();
				templateJs = templateJs.Replace("#EntityName", viewModel.TableModel.Name);
				templateJs = templateJs.Replace("#EntityId", viewModel.TableModelId.ToString());
				templateJs = templateJs.Replace("#ViewModelId", viewModel.Id.ToString());
				templateJs = templateJs.Replace("#ListId", listId.ToString());
				var res = _pageRender.SavePageContent(pageId, template, "", templateJs);

				if (!res.IsSuccess)
				{

				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				return Json(default(ResultModel));
			}

			await Notify.SendNotificationAsync(new SystemNotifications
			{
				Content = $"New page generated with name {page.Settings.Name}  and route {page.Path}",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});

			return Json(new ResultModel
			{
				IsSuccess = true,
				Result = pageId
			});
		}
		private const string BasePath = "Static/Templates/";
	}
}