using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ST.BaseBusinessRepository;
using ST.Configuration;
using ST.Configuration.Seed;
using ST.Configuration.ViewModels.LocalizationViewModels;
using ST.CORE.Attributes;
using ST.CORE.Services.Abstraction;
using ST.CORE.ViewModels;
using ST.CORE.ViewModels.PageViewModels;
using ST.CORE.ViewModels.TableColumnsViewModels;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Localization;

namespace ST.CORE.Controllers.Render
{
	[Authorize]
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
		private readonly IDynamicService _service;
		/// <summary>
		/// Inject loc config
		/// </summary>
		private readonly IOptionsSnapshot<LocalizationConfigModel> _locConfig;
		/// <summary>
		/// Inject localize
		/// </summary>
		private readonly IStringLocalizer _localize;
		/// <summary>
		/// Inject page render
		/// </summary>
		private readonly IPageRender _pageRender;

		/// <summary>
		/// Inject menu dataService
		/// </summary>
		private readonly IMenuService _menuService;

		/// <summary>
		/// Inject user manager
		/// </summary>
		private readonly UserManager<ApplicationUser> _userManager;

		/// <summary>
		/// Inject iso dataService
		/// </summary>
		private readonly ITreeIsoService _isoService;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="appContext"></param>
		/// <param name="service"></param>
		/// <param name="localize"></param>
		/// <param name="locConfig"></param>
		/// <param name="pageRender"></param>
		/// <param name="menuService"></param>
		/// <param name="userManager"></param>
		/// <param name="isoService"></param>
		public PageRenderController(EntitiesDbContext context, ApplicationDbContext appContext,
			IDynamicService service, IStringLocalizer localize,
			IOptionsSnapshot<LocalizationConfigModel> locConfig,
			IPageRender pageRender,
			IMenuService menuService, UserManager<ApplicationUser> userManager, ITreeIsoService isoService)
		{
			_context = context;
			_appContext = appContext;
			_service = service;
			_localize = localize;
			_locConfig = locConfig;
			_menuService = menuService;
			_userManager = userManager;
			_isoService = isoService;
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
		/// Get entity fields
		/// </summary>
		/// <param name="tableId"></param>
		/// <returns></returns>
		[HttpGet]
		[Authorize(Roles = Settings.SuperAdmin)]
		public JsonResult GetEntityFields(Guid tableId)
		{
			var fields = _context.Table
				.Include(x => x.TableFields)
				.FirstOrDefault(x => !x.IsDeleted && x.Id == tableId)?.TableFields
				.Select(x => new
				{
					x.Id,
					x.Name,
					x.DataType
				})
				.ToList();

			return new JsonResult(fields);
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
			var instance = _service.Table(table);
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
		[HttpGet]
		[AjaxOnly]
		[Authorize(Roles = Settings.SuperAdmin)]
		public JsonResult GetJsonExampleOfEntity([Required] Guid viewModelId)
		{
			var entity = _context.ViewModels.Include(x => x.TableModel).FirstOrDefault(x => x.Id.Equals(viewModelId));
			if (entity == null) return Json(default(ResultModel));
			var obj = _service.Table(entity.TableModel.Name).Object;
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

			var translations = _localize.GetAllForLanguage(lang);
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
		[Authorize(Roles = Settings.SuperAdmin)]
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
		[Authorize(Roles = Settings.SuperAdmin)]
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
			if (table == null) return Json(null);
			var instance = _service.Table(table.Name);
			return Json(await instance.GetAll<object>());

		}

		/// <summary>
		/// Load paged data with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <param name="viewModelId"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public async Task<JsonResult> LoadPagedData(DTParameters param, Guid viewModelId)
		{
			if (viewModelId == Guid.Empty) return Json(default(DTResult<object>));
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
						? $"{column.Name ?? field.Name} {param.SortOrder}"
						: $"{field.Name} {param.SortOrder}";
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			var roles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User));

			var (data, recordsCount) = await _service.Filter(viewModel.TableModel.Name, param.Search.Value, sortColumn, param.Start,
				param.Length, x => x.SortByUserRoleAccess(roles, Settings.SuperAdmin));

			var finalResult = new DTResult<object>
			{
				draw = param.Draw,
				data = data,
				recordsFiltered = recordsCount,
				recordsTotal = data.Count()
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
		/// Get all pages
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public JsonResult GetPages()
		{
			var pages = _context.Pages
				.Include(x => x.Settings)
				.Where(x => !x.IsDeleted && !x.IsLayout)
				.Select(x => new
				{
					Id = x.Path,
					x.Settings.Name
				}).ToList();

			return new JsonResult(pages);
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
			var list = await _service.Table(entityName).GetAll<object>();
			return new JsonResult(list.Result);
		}

		/// <summary>
		/// Get all languages
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
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
			if (table == null) return Json(result);
			{
				var instance = _service.Table(table.Name);
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
		/// <param name="viewModelId"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		[AjaxOnly]
		[Authorize(Roles = Settings.SuperAdmin)]
		public async Task<JsonResult> DeleteItemFromDynamicEntity(Guid viewModelId, string id)
		{
			if (string.IsNullOrEmpty(id) || viewModelId == Guid.Empty) return Json(new { message = "Fail to delete!", success = false });
			var viewModel = _context.ViewModels.Include(x => x.TableModel).FirstOrDefault(x => x.Id.Equals(viewModelId));
			if (viewModel == null) return Json(new { message = "Fail to delete!", success = false });
			var response = await _service.Table(viewModel.TableModel.Name).Delete<object>(Guid.Parse(id));
			if (!response.IsSuccess) return Json(new { message = "Fail to delete!", success = false });

			return Json(new { message = "Item was deleted!", success = true });
		}

		/// <summary>
		/// Delete page by id
		/// </summary>
		/// <param name="viewModelId"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		[AjaxOnly]
		[Authorize(Roles = Settings.SuperAdmin)]
		public async Task<JsonResult> RestoreItemFromDynamicEntity(Guid viewModelId, string id)
		{
			if (string.IsNullOrEmpty(id) || viewModelId == Guid.Empty) return Json(new { message = "Fail to restore!", success = false });
			var viewModel = _context.ViewModels.Include(x => x.TableModel).FirstOrDefault(x => x.Id.Equals(viewModelId));
			if (viewModel == null) return Json(new { message = "Fail to restore!", success = false });
			var response = await _service.Table(viewModel.TableModel.Name).Restore<object>(Guid.Parse(id));
			if (!response.IsSuccess) return Json(new { message = "Fail to restore!", success = false });

			return Json(new { message = "Item was restored!", success = true });
		}


		/// <summary>
		/// Load tree for standards
		/// </summary>
		/// <param name="standardEntityId"></param>
		/// <param name="categoryEntityId"></param>
		/// <param name="requirementEntityId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetTreeData(Guid? standardEntityId, Guid? categoryEntityId, Guid? requirementEntityId)
		{
			var result = new ResultModel();
			if (standardEntityId == null || categoryEntityId == null || requirementEntityId == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(Guid.NewGuid().ToString(), "Tree block configuration is not complete!")
				};
				return Json(result);
			}
			var standardEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == standardEntityId);
			var categoryEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == categoryEntityId);
			var requirementEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == requirementEntityId);
			if (standardEntity == null || categoryEntity == null || requirementEntity == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(Guid.NewGuid().ToString(), "Entities does not exist!")
				};
				return Json(result);
			}

			return Json(await _isoService.LoadTreeStandard(standardEntity, categoryEntity, requirementEntity));
		}

		/// <summary>
		/// Get view model column type for inline table edit
		/// </summary>
		/// <param name="viewModelId"></param>
		/// <returns></returns>
		public async Task<JsonResult> GetViewModelColumnTypes(Guid? viewModelId)
		{
			var result = new ResultModel();
			if (viewModelId == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "Not specified view model id")
				};
				Json(result);
			}

			var viewModel = await _context.ViewModels.Include(x => x.ViewModelFields).FirstOrDefaultAsync(x => x.Id == viewModelId);
			if (viewModel == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "ViewModel not found")
				};
				Json(result);
			}

			var tableFields = _context.TableFields.Where(x => x.TableId == viewModel.TableModelId).ToList();
			var res = new List<TableColumnData>();
			foreach (var field in tableFields)
			{
				var obj = field.Adapt<TableColumnData>();
				obj.ColumnId = viewModel?.ViewModelFields?.FirstOrDefault(x => x.TableModelFieldsId == field.Id)?.Id;
				res.Add(obj);
			}

			if (!tableFields.Any()) return Json(result);
			result.IsSuccess = true;
			result.Result = res;

			return Json(result);
		}

		/// <summary>
		/// Save table cell
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="propertyId"></param>
		/// <param name="rowId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public async Task<JsonResult> SaveTableCellData(Guid? entityId, Guid? propertyId, Guid? rowId, string value)
		{
			var result = new ResultModel();
			if (entityId == null || propertyId == null || rowId == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "Not specified data")
				};
				return Json(result);
			}

			var entity = await _context.Table.Include(x => x.TableFields).FirstOrDefaultAsync(x => x.Id == entityId);
			if (entity == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "Entity not found")
				};
				return Json(result);
			}

			if (entity.IsSystem || entity.IsPartOfDbContext)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "The system entity can not be edited")
				};
				return Json(result);
			}

			var property = entity.TableFields.First(x => x.Id == propertyId);
			if (property == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "Not found entity column")
				};
				return Json(result);
			}

			var row = await _service.Table(entity.Name).GetById<object>(rowId.Value);
			if (!row.IsSuccess)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "Entry Not found")
				};
				return Json(result);
			}

			row.Result.ChangePropValue(property.Name, value);
			row.Result.ChangePropValue("Changed", DateTime.Now.ToString(CultureInfo.InvariantCulture));

			var req = await _service.Table(entity.Name).Update(row.Result);
			if (!req.IsSuccess)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(string.Empty, "Fail to save data")
				};
				return Json(result);
			}

			result.IsSuccess = true;
			return Json(result);
		}
	}
}