using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.Installation;
using ST.CORE.Models;
using ST.CORE.Models.LocalizationViewModels;
using ST.CORE.Services.Abstraction;
using ST.CORE.ViewModels.Pages;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Services.Abstraction;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Localization;

namespace ST.CORE.Controllers.Render
{
	public class PageRenderController : Controller
	{
		#region InjectRegion
		/// <summary>
		/// DB context
		/// </summary>
		private readonly EntitiesDbContext _context;
		/// <summary>
		/// App Context
		/// </summary>
		private readonly ApplicationDbContext _appContext;
		/// <summary>
		/// Inject Data Service
		/// </summary>
		private readonly IDynamicEntityDataService _dataService;
		/// <summary>
		/// Inject loc config
		/// </summary>
		private readonly IOptionsSnapshot<LocalizationConfigModel> _locConfig;
		/// <summary>
		/// Inject localizer
		/// </summary>
		private readonly IStringLocalizer _localizer;
		/// <summary>
		/// Inject Service
		/// </summary>
		private readonly ILocalizationService _localizationService;
		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		/// <summary>
		/// Inject menu service
		/// </summary>
		private readonly IMenuService _menuService;

		/// <summary>
		/// Inject user manager
		/// </summary>
		private readonly UserManager<ApplicationUser> _userManager;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="appContext"></param>
		/// <param name="dataService"></param>
		/// <param name="localizer"></param>
		/// <param name="locConfig"></param>
		/// <param name="pageRender"></param>
		/// <param name="localizationService"></param>
		/// <param name="menuService"></param>
		/// <param name="userManager"></param>
		public PageRenderController(EntitiesDbContext context, ApplicationDbContext appContext,
			IDynamicEntityDataService dataService, IStringLocalizer localizer,
			IOptionsSnapshot<LocalizationConfigModel> locConfig,
			IPageRender pageRender,
			ILocalizationService localizationService, IMenuService menuService, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_appContext = appContext;
			_dataService = dataService;
			_localizer = localizer;
			_locConfig = locConfig;
			_localizationService = localizationService;
			_menuService = menuService;
			_userManager = userManager;
			_pageRender = pageRender;
		}

		/// <summary>
		/// Get page by page name
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		public IActionResult Index([Required]Guid pageId)
		{
			var page = _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(pageId));
			if (page == null) return NotFound();
			ViewBag.Page = page;

			return View();
		}

		/// <summary>
		/// Get All Entities
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetEntities()
		{
			var entities = _context.Table.Where(x => !x.IsDeleted).ToList();

			return new JsonResult(entities);
		}

		/// <summary>
		/// Get All Entities
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetRoles()
		{
			var roles = _appContext.Roles.Where(x => !x.IsDeleted).ToList();

			return new JsonResult(roles);
		}

		/// <summary>
		/// Get All view Models
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetViewModels()
		{
			var viewModels = _context.ViewModels.Where(x => !x.IsDeleted).ToList();

			return new JsonResult(viewModels);
		}

		/// <summary>
		/// Get input select values
		/// </summary>
		/// <param name="fieldId"></param>
		/// <returns></returns>
		public async Task<JsonResult> GetInputSelectValues(Guid fieldId)
		{
			if (Guid.Empty == fieldId) return Json(new ResultModel());

			var field = _context.TableFields.FirstOrDefault(x => x.Id.Equals(fieldId));
			if (field == null) return Json(new ResultModel());
			var config = _context.TableFieldConfigValues
				.Include(x => x.TableFieldConfig)
				.ThenInclude(x => x.TableFieldType)
				.FirstOrDefault(x => x.TableModelFieldId.Equals(fieldId));
			if (config == null) return Json(new ResultModel());
			if (!config.TableFieldConfig.TableFieldType.Name.Equals("EntityReference")) return Json(new ResultModel());
			var table = config.Value;
			var instance = _dataService.Table(table);
			return Json(await instance.GetAll<object>());
		}

		/// <summary>
		/// Get view model by id
		/// </summary>
		/// <param name="viewModelId"></param>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetViewModelById([Required]Guid viewModelId)
		{
			var obj = _context.ViewModels
				.Include(x => x.TableModel)
				.Include(x => x.ViewModelFields)
				.FirstOrDefault(x => !x.IsDeleted && x.Id.Equals(viewModelId));

			if (obj != null)
			{
				obj.ViewModelFields = obj.ViewModelFields.OrderBy(x => x.Order);
			}

			return Json(new ResultModel
			{
				IsSuccess = true,
				Result = obj
			});
		}

		/// <summary>
		/// Get example object of entity
		/// </summary>
		/// <param name="viewModelId"></param>
		/// <returns></returns>
		public JsonResult GetJsonExampleOfEntity([Required] Guid viewModelId)
		{
			var entity = _context.ViewModels.Include(x => x.TableModel).FirstOrDefault(x => x.Id.Equals(viewModelId));
			if (entity == null) return Json(default(ResultModel));
			var obj = _dataService.Table(entity.TableModel.Name).Object;
			return Json(new { row = obj });
		}

		/// <summary>
		/// Get all blocks
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetBlocks()
		{
			var blocks = _context.Blocks.Include(x => x.BlockCategory).Where(x => !x.IsDeleted).ToList();
			var result = blocks.Select(x => new
			{
				x.Id,
				BlockName = x.Name,
				Html = x.HtmlCode,
				Css = x.CssCode,
				Category = x.BlockCategory.Name,
				Icon = x.FaIcon
			});
			return new JsonResult(result);
		}

		/// <summary>
		/// Get translations
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetTranslations()
		{
			var lang = HttpContext.Session.GetString("lang");
			var languages = _locConfig.Value.Languages.Select(x => x.Identifier);
			if (!languages.Contains(lang))
			{
				return Json(null);
			}

			var translations = _localizer.GetAllForLanguage(lang);
			var json = translations.ToDictionary(trans => trans.Name, trans => trans.Value);
			return Json(json);
		}

		/// <summary>
		/// Get scripts as list for page by id
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetScripts(Guid pageId)
		{
			if (pageId == Guid.Empty) return Json(default(IEnumerable<string>));
			var scripts = new HashSet<string>();
			var page = _context.Pages.Include(x => x.PageScripts).FirstOrDefault(x => x.Id.Equals(pageId));

			if (page == null) return Json(default(IEnumerable<string>));

			if (!page.IsLayout && page.LayoutId != null)
			{

				var layout = _context.Pages.Include(x => x.PageScripts).FirstOrDefault(x => x.Id.Equals(page.LayoutId));

				if (layout != null && layout.PageScripts.Any())
				{
					var extracted = layout.PageScripts.OrderBy(x => x.Order).Where(x => !string.IsNullOrEmpty(x.Script))
						.Select(x => ExtractSrcFromScript(x.Script)).ToList();
					extracted.ForEach(x => { scripts.Add(x); });
				}
			}

			if (!page.PageScripts.Any()) return new JsonResult(scripts.ToList());
			{
				var extracted = page.PageScripts.OrderBy(x => x.Order).Where(x => !string.IsNullOrEmpty(x.Script))
					.Select(x => ExtractSrcFromScript(x.Script)).ToList();
				extracted.ForEach(x => { scripts.Add(x); });
			}

			return new JsonResult(scripts.ToList());
		}


		/// <summary>
		/// Get scripts as list for page by id
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetStyles(Guid pageId)
		{
			if (pageId == Guid.Empty) return Json(default(IEnumerable<string>));
			var styles = new HashSet<string>();
			var page = _context.Pages.Include(x => x.PageStyles).FirstOrDefault(x => x.Id.Equals(pageId));

			if (page == null) return Json(default(IEnumerable<string>));

			if (!page.IsLayout && page.LayoutId != null)
			{

				var layout = _context.Pages.Include(x => x.PageStyles).FirstOrDefault(x => x.Id.Equals(page.LayoutId));

				if (layout != null && layout.PageStyles.Any())
				{
					var extracted = layout.PageStyles.OrderBy(x => x.Order).Where(x => !string.IsNullOrEmpty(x.Script))
						.Select(x => x.Script).ToList();
					extracted.ForEach(x => { styles.Add(x); });
				}
			}

			if (!page.PageStyles.Any()) return new JsonResult(styles.ToList());
			{
				var extracted = page.PageStyles.OrderBy(x => x.Order).Where(x => !string.IsNullOrEmpty(x.Script))
					.Select(x => x.Script).ToList();
				extracted.ForEach(x => { styles.Add(x); });
			}

			return new JsonResult(styles.ToList());
		}

		/// <summary>
		/// Extract Src from script
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		private static string ExtractSrcFromScript(string script)
		{
			try
			{
				var sp = script.Split("src=\"")?[1].Split("\"").FirstOrDefault();
				return sp;
			}
			catch
			{
				//Ignore
			}
			return string.Empty;
		}

		/// <summary>
		/// Get menu item roles 
		/// </summary>
		/// <param name="menuId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetMenuItemRoles([Required]Guid menuId)
		{
			if (menuId == Guid.Empty) return Json(new ResultModel());
			var roles = await _menuService.GetMenuRoles(menuId);

			return Json(roles);
		}

		/// <summary>
		/// Get menus
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetMenus(Guid? menuId = null)
		{
			if (menuId == null)
			{
				menuId = MenuSync.NavBarId;
			}

			var user = await _userManager.GetUserAsync(User);
			var roles = await _userManager.GetRolesAsync(user);
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
		public async Task<JsonResult> UpdateMenuItemRoleAccess([Required]Guid menuId, IList<string> roles)
		{
			return Json(await _menuService.UpdateMenuItemRoleAccess(menuId, roles));
		}


		/// <summary>
		/// Get list data by entity id
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetListData(Guid entityId)
		{
			var table = _context.Table.Include(x => x.TableFields)
				.FirstOrDefault(x => x.Id.Equals(entityId));
			if (table != null)
			{
				var instance = _dataService.Table(table.Name);
				return Json(await instance.GetAll<object>());
			}

			return Json(null);
		}

		/// <summary>
		/// Load paged data with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <param name="viewModelId"></param>
		/// <param name="tableId"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public async Task<JsonResult> LoadPagedData(DTParameters param, Guid viewModelId, Guid tableId)
		{
			if (viewModelId == Guid.Empty || tableId == Guid.Empty) return Json(default(DTResult<object>));
			var viewModel = await _context.ViewModels
				.Include(x => x.TableModel)
				.ThenInclude(x => x.TableFields)
				.Include(x => x.ViewModelFields)
				.ThenInclude(x => x.TableModelFields)
				.FirstOrDefaultAsync(x => x.Id.Equals(viewModelId));

			if (viewModel == null) return Json(default(DTResult<object>));
			var sortColumn = param.SortOrder;
			try
			{
				var columnIndex = Convert.ToInt32(param.Order[0].Column);
				var field = viewModel.ViewModelFields.ElementAt(columnIndex);
				if (field != null)
				{
					var column =
						viewModel.TableModel.TableFields.FirstOrDefault(x => x.Id == field.TableModelFieldsId);
					sortColumn = column != null
						? $"{column?.Name ?? field.Name} {param.SortOrder}"
						: $"{field.Name} {param.SortOrder}";
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			var (objects, item2) = await _dataService.Filter(viewModel.TableModel.Name, param.Search.Value, sortColumn, param.Start,
				param.Length);

			var finalResult = new DTResult<object>
			{
				draw = param.Draw,
				data = objects,
				recordsFiltered = item2,
				recordsTotal = objects.Count()
			};

			return Json(finalResult);
		}

		/// <summary>
		/// Get all forms
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetForms()
		{
			var forms = _context.Forms.Where(x => !x.IsDeleted).ToList();

			return new JsonResult(forms);
		}

		/// <summary>
		/// Load data from entity
		/// </summary>
		/// <param name="entityName"></param>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpGet]
		public async Task<JsonResult> LoadDataFromEntity(string entityName)
		{
			var list = await _dataService.Table(entityName).GetAll<object>();
			return new JsonResult(list.Result);
		}

		/// <summary>
		/// Get all languages
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetLanguages()
		{
			var languages = _locConfig.Value.Languages.ToList();
			return Json(languages);
		}

		/// <summary>
		/// Get page script
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		[HttpGet]
		public string GetPageScript(Guid pageId)
		{
			if (pageId == Guid.Empty) return string.Empty;
			var page = _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(pageId));
			if (page == null) return string.Empty;
			var script = _pageRender.GetPageJavaScript(page.Settings.Name);
			return script;
		}
		/// <summary>
		/// Post Form
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> PostForm(PostFormViewModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false
			};

			if (model == null) return Json(result);
			var form = _context.Forms.FirstOrDefault(x => x.Id.Equals(model.FormId));
			if (form == null) return Json(result);

			var table = _context.Table.Include(x => x.TableFields)
				.FirstOrDefault(x => x.Id.Equals(form.TableId));
			if (table != null)
			{
				var instance = _dataService.Table(table.Name);
				var fields = table.TableFields.ToList();
				var pre = instance.Object;

				foreach (var (key, value) in model.Data)
				{
					var field = fields.FirstOrDefault(x => x.Id.Equals(Guid.Parse(key)));
					if (field == null) continue;
					try
					{
						var prop = pre.GetType().GetProperty(field.Name);
						if (prop.PropertyType == typeof(Guid))
						{
							prop.SetValue(pre, Guid.Parse(value));
						}
						else
						{
							prop.SetValue(pre, value);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				}

				var obj = instance.ParseObject(pre);

				try
				{
					obj.GetType().GetProperty("Author").SetValue(obj, User.Identity.Name);
					obj.GetType().GetProperty("ModifiedBy").SetValue(obj, User.Identity.Name);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

				var req = await instance.Add(obj);

				if (req.IsSuccess)
				{
					return Json(new ResultModel
					{
						IsSuccess = true,
						Result = new
						{
							IdOfCreatedObject = req.Result,
							form.RedirectUrl
						}
					});
				}
			}

			return Json(result);
		}


		/// <summary>
		/// Delete page by id
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		public async Task<JsonResult> DeleteItemFromDynamicEntity(Guid entityId, string id)
		{
			if (string.IsNullOrEmpty(id) || entityId == Guid.Empty) return Json(new { message = "Fail to delete!", success = false });
			var table = _context.Table.FirstOrDefault(x => x.Id.Equals(entityId));
			if (table == null) return Json(new { message = "Fail to delete!", success = false });
			var response = await _dataService.Table(table.Name).Delete<object>(Guid.Parse(id));
			if (!response.IsSuccess) return Json(new { message = "Fail to delete!", success = false });

			return Json(new { message = "Item was deleted!", success = true });
		}

		public async Task<JsonResult> GetTreeData(Guid? standartEntityId, Guid? categoryEntityId, Guid? requirementEntityId)
		{
			var result = new ResultModel();
			if (standartEntityId == null || categoryEntityId == null || requirementEntityId == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(Guid.NewGuid().ToString(), "Tree block configuration is not complete!")
				};
				return Json(result);
			}
			var standartEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == standartEntityId);
			var categoryEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == categoryEntityId);
			var requirementEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == requirementEntityId);

			if (standartEntity == null || categoryEntity == null || requirementEntity == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(Guid.NewGuid().ToString(), "Entities does not exist!")
				};
				return Json(result);
			}

			var standarts = await _dataService.Table(standartEntity.Name).GetAll<dynamic>();

			return Json(null);
		}
	}
}