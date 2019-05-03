using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ST.Audit.Extensions;
using ST.Configuration.Services.Abstraction;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Entities.Data;
using ST.Entities.Settings;
using ST.Identity.Data;
using ST.PageRender.Razor.Extensions;
using ST.PageRender.Razor.Helpers;
using ST.PageRender.Razor.Services.Abstractions;
using ST.PageRender.Razor.ViewModels.PageViewModels;
using ST.PageRender.Razor.ViewModels.TableColumnsViewModels;
using ST.Core;
using ST.Core.Attributes;
using ST.Core.Helpers;
using ST.Identity.Abstractions;

namespace ST.PageRender.Razor.Controllers
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

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appContext"></param>
        /// <param name="service"></param>
        /// <param name="pageRender"></param>
        /// <param name="menuService"></param>
        /// <param name="userManager"></param>
        public PageRenderController(EntitiesDbContext context, ApplicationDbContext appContext,
            IDynamicService service,
            IPageRender pageRender,
            IMenuService menuService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appContext = appContext;
            _service = service;
            _menuService = menuService;
            _userManager = userManager;
            _pageRender = pageRender;
        }

        /// <summary>
        /// Get page by page name
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index([Required]Guid pageId)
        {
            var page = await _pageRender.GetPageAsync(pageId);
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
            var config = await _context.TableFieldConfigValues
                .Include(x => x.TableFieldConfig)
                .ThenInclude(x => x.TableFieldType)
                .FirstOrDefaultAsync(x => x.TableModelFieldId.Equals(fieldId) && x.TableFieldConfig.Code == "3000");
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
                .ThenInclude(x => x.TableModelFields)
                .FirstOrDefault(x => !x.IsDeleted && x.Id.Equals(viewModelId));

            if (obj != null)
            {
                obj.ViewModelFields = obj.ViewModelFields.OrderBy(x => x.Order);
            }

            return Json(new ResultModel
            {
                IsSuccess = true,
                Result = obj
            }, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
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
            var objType = _service.Table(entity.TableModel.Name).Type;
            var obj = Activator.CreateInstance(objType);
            var referenceFields = obj.GetType().GetProperties()
                .Where(x => !x.PropertyType.GetTypeInfo().FullName.StartsWith("System"))
                .ToList();
            foreach (var refField in referenceFields)
            {
                var refPropName = refField.Name;
                try
                {
                    var refType = obj.GetType().GetProperty(refPropName).PropertyType;
                    var newInstance = Activator.CreateInstance(refType);
                    obj.GetType().GetProperty(refPropName).SetValue(obj, newInstance);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

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
                menuId = MenuManager.NavBarId;
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
                Draw = param.Draw,
                Data = data,
                RecordsFiltered = recordsCount,
                RecordsTotal = data.Count()
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
        /// Get page script
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetPageScript(Guid pageId)
        {
            if (pageId == Guid.Empty) return string.Empty;
            var page = await _pageRender.GetPageAsync(pageId);
            return page == null ? string.Empty : page.Settings?.JsCode;
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
                IsSuccess = false,
                Errors = new List<IErrorModel>()
            };

            if (model == null)
            {
                result.Errors.Add(new ErrorModel("Null", "Data is not defined!"));
                return Json(result);
            }
            var form = _context.Forms.FirstOrDefault(x => x.Id.Equals(model.FormId));
            if (form == null)
            {
                result.Errors.Add(new ErrorModel("Null", "Form not found!"));
                return Json(result);
            }

            var table = _context.Table.Include(x => x.TableFields)
                .FirstOrDefault(x => x.Id.Equals(form.TableId));
            if (table == null)
            {
                result.Errors.Add(new ErrorModel("Null", "Form entity reference not found"));
                return Json(result);
            }

            if (model.IsEdit && !model.SystemFields.Any())
            {
                result.Errors
                    .Add(new ErrorModel("Fail", "No object id passed on form, try to refresh page and try again"));
                return Json(result);
            }

            var id = model.SystemFields?.FirstOrDefault(x => x.Key == "Id");

            var instance = _service.Table(table.Name);
            var fields = table.TableFields.ToList();
            var obj = Activator.CreateInstance(instance.Type);

            if (model.IsEdit)
            {
                if (id == null)
                {
                    result.Errors
                    .Add(new ErrorModel("Fail", "No object id passed on form, try to refresh page and try again"));
                    return Json(result);
                }
                var oldObj = await instance.GetById<object>(id.Value.ToGuid());
                if (!oldObj.IsSuccess)
                {
                    result.Errors
                    .Add(new ErrorModel("Fail", "Data missed, check if this data exist!"));
                    return Json(result);
                }
                obj = oldObj.Result;
            }

            foreach (var (key, value) in model.Data)
            {
                var field = fields.FirstOrDefault(x => x.Id.Equals(Guid.Parse(key)));
                if (field == null) continue;
                try
                {
                    var prop = obj.GetType().GetProperty(field.Name);
                    if (prop.PropertyType == typeof(Guid))
                    {
                        if (value == null)
                        {
                            prop.SetValue(obj, null);
                        }
                        else
                        {
                            prop.SetValue(obj, Guid.Parse(value));
                        }
                    }
                    else
                    {
                        prop.SetValue(obj, value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            var req = (model.IsEdit) ? await instance.Update(obj) : await instance.Add(obj);

            if (req.IsSuccess)
            {
                result.IsSuccess = true;
                result.Result = new
                {
                    IdOfCreatedObject = req.Result,
                    form.RedirectUrl
                };
                return Json(result);
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
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        [AjaxOnly]
        [Authorize(Roles = Settings.SuperAdmin)]
        public async Task<JsonResult> DeleteItemsFromDynamicEntity(Guid viewModelId, IEnumerable<string> ids)
        {
            if (ids == null) return Json(new { message = "Fail to delete!", success = false });
            var viewModel = _context.ViewModels.Include(x => x.TableModel).FirstOrDefault(x => x.Id.Equals(viewModelId));
            if (viewModel == null) return Json(new { message = "Fail to delete!", success = false });
            foreach (var id in ids)
            {
                await _service.Table(viewModel.TableModel.Name).Delete<object>(Guid.Parse(id));
            }

            return Json(new { message = "Items was deleted!", success = true });
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
        /// Get row select references
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [AjaxOnly]
        [HttpGet]
        public async Task<JsonResult> GetRowReferences([Required]Guid entityId, [Required]Guid propertyId)
        {
            var response = new ResultModel();
            var refProp = _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefault(x => x.Id == entityId)?
                .TableFields?.FirstOrDefault(x => x.Id == propertyId);
            if (refProp == null)
            {
                response.Errors = new List<IErrorModel>
                {
                    new ErrorModel("fail", "Property reference not found")
                };
                return Json(response);
            }

            var entityRefName = refProp.TableFieldConfigValues.FirstOrDefault(x => x.TableFieldConfig.Code == "3000");
            if (entityRefName == null)
            {
                response.Errors = new List<IErrorModel>
                {
                    new ErrorModel("fail", "Property reference not found")
                };
                return Json(response);
            }

            var res = await _service.GetAll(entityRefName.Value);
            response.IsSuccess = res.IsSuccess;
            response.Result = new
            {
                Data = res.Result,
                EntityName = entityRefName.Value
            };
            return Json(response);
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

            var row = await _service.GetById(entity.Name, rowId.Value);
            if (!row.IsSuccess)
            {
                result.Errors = new List<IErrorModel>
                {
                    new ErrorModel(string.Empty, "Entry Not found")
                };
                return Json(result);
            }

            if (row.Result.ContainsKey(property.Name))
            {
                switch (property.DataType)
                {
                    case TableFieldDataType.Guid:
                        {
                            Guid.TryParse(value, out var parsed);
                            row.Result[property.Name] = parsed;
                        }
                        break;
                    case TableFieldDataType.Boolean:
                        {
                            bool.TryParse(value, out var val);
                            row.Result[property.Name] = val;
                        }
                        break;
                    case TableFieldDataType.Int:
                        {
                            try
                            {
                                row.Result[property.Name] = Convert.ToInt32(value);
                            }
                            catch
                            {
                                row.Result[property.Name] = value;
                            }
                        }
                        break;
                    case TableFieldDataType.Decimal:
                        {
                            try
                            {
                                row.Result[property.Name] = Convert.ToDecimal(value);
                            }
                            catch
                            {
                                row.Result[property.Name] = value;
                            }
                        }
                        break;
                    case TableFieldDataType.Date:
                    case TableFieldDataType.DateTime:
                        {
                            DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed);
                            row.Result[property.Name] = parsed;
                        }
                        break;
                    default:
                        row.Result[property.Name] = value;
                        break;
                }
            }

            if (row.Result.ContainsKey(nameof(BaseModel.Changed)))
            {
                row.Result[nameof(BaseModel.Changed)] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }

            var req = await _service.Update(entity.Name, row.Result);
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