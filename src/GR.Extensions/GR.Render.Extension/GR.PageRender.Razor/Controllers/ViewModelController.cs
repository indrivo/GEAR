using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Constants;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Configurations;
using GR.PageRender.Abstractions.Models.ViewModels;

namespace GR.PageRender.Razor.Controllers
{
    [Authorize]
    public class ViewModelController : Controller
    {
        /// <summary>
        /// Inject page render
        /// </summary>
        private readonly IPageRender _pageRender;

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pageRender"></param>
        /// <param name="pagesContext"></param>
        public ViewModelController(IPageRender pageRender, IDynamicPagesContext pagesContext)
        {
            _pageRender = pageRender;
            _pagesContext = pagesContext;
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
        /// Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var viewModel = _pagesContext.ViewModels.FirstOrDefault(x => x.Id.Equals(id));
            if (viewModel == null) return NotFound();
            return View(viewModel);
        }

        /// <summary>
        /// Edit template of view model field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult TemplateEdit([Required] Guid id)
        {
            var viewModelField = _pagesContext.ViewModelFields.FirstOrDefault(x => x.Id.Equals(id));
            if (viewModelField == null) return NotFound();
            return View(viewModelField);
        }

        /// <summary>
        /// Template edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TemplateEdit(ViewModelFields model)
        {
            if (model.Id != Guid.Empty)
            {
                var dataModel = _pagesContext.ViewModelFields.Include(x => x.ViewModel).FirstOrDefault(x => x.Id.Equals(model.Id));
                if (dataModel == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid data entry!");
                    return View(model);
                }

                dataModel.Template = model.Template;
                _pagesContext.ViewModelFields.Update(dataModel);
                try
                {
                    _pagesContext.SaveChanges();
                    return RedirectToAction("OrderFields", new { dataModel.ViewModel.Id });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid data entry!");
            return View(model);
        }


        /// <summary>
        /// Update view model name
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit([Required] ViewModel model)
        {
            if (model.Id != Guid.Empty)
            {
                var dataModel = _pagesContext.ViewModels.FirstOrDefault(x => x.Id.Equals(model.Id));
                if (dataModel == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid data entry!");
                    return View(model);
                }

                dataModel.Name = model.Name;
                _pagesContext.ViewModels.Update(dataModel);
                try
                {
                    _pagesContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid data entry!");
            return View(model);
        }


        /// <summary>
        /// Create view model
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GenerateViewModel([Required] Guid entityId)
        {
            return Json(await _pageRender.GenerateViewModel(entityId));
        }

        /// <summary>
        /// Update styles of page
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateViewModelFields([Required] IEnumerable<ViewModelFields> items)
        {
            return UpdateItems(items.ToList());
        }

        /// <summary>
        /// Update items
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        [NonAction]
        private JsonResult UpdateItems<TItem>(IList<TItem> items) where TItem : ViewModelFields
        {
            var result = new ResultModel();
            if (!items.Any())
            {
                result.IsSuccess = true;
                return Json(result);
            }

            var pageId = items.First().ViewModelId;
            var pageScripts = _pagesContext.SetEntity<TItem>().Where(x => x.ViewModelId.Equals(pageId)).ToList();

            foreach (var prev in pageScripts)
            {
                var up = items.FirstOrDefault(x => x.Id.Equals(prev.Id));
                if (up == null)
                {
                    _pagesContext.SetEntity<TItem>().Remove(prev);
                }
                else if (prev.Order != up.Order || prev.Name != up.Name)
                {
                    prev.Name = up.Name;
                    prev.Order = up.Order;
                    _pagesContext.SetEntity<TItem>().Update(prev);
                }
            }

            var news = items.Where(x => x.Id == Guid.Empty).Select(x => new
            {
                ViewModelId = pageId,
                x.Name,
                x.Order
            }).Adapt<IEnumerable<TItem>>().ToList();

            if (news.Any())
            {
                _pagesContext.SetEntity<TItem>().AddRange(news);
            }

            try
            {
                _pagesContext.SaveChanges();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new JsonResult(result);
        }

        /// <summary>
        /// Order fields
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OrderFields([Required] Guid id)
        {
            if (Guid.Empty == id) return NotFound();
            var viewModel = await _pagesContext.ViewModels
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.TableModelFields)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (viewModel == null) return NotFound();
            ViewBag.ViewModel = viewModel;
            var model = viewModel.ViewModelFields
                .OrderBy(x => x.Order).ToList();
            return View(model);
        }


        /// <summary>
        /// Load page types with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadViewModels(DTParameters param, Guid entityId)
        {
            var filtered = _pagesContext.FilterAbstractContext<ViewModel>(param.Search.Value, param.SortOrder,
                param.Start,
                param.Length,
                out var totalCount,
                x => (entityId != Guid.Empty && x.TableModelId == entityId) || entityId == Guid.Empty);


            var sel = filtered.Select(x => new
            {
                x.Author,
                x.Changed,
                x.Created,
                x.Id,
                x.IsDeleted,
                x.ModifiedBy,
                x.Name,
                Table = _pagesContext.Table.FirstOrDefault(y => y.Id.Equals(x.TableModelId))?.Name
            }).Adapt<IEnumerable<object>>();

            var finalResult = new DTResult<object>
            {
                Draw = param.Draw,
                Data = sel.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count()
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
        public JsonResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete view model!", success = false });
            var page = _pagesContext.ViewModels.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
            if (page == null) return Json(new { message = "Fail to delete view model!", success = false });

            try
            {
                _pagesContext.ViewModels.Remove(page);
                _pagesContext.SaveChanges();
                return Json(new { message = "View model was delete with success!", success = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Json(new { message = "Fail to delete view model!", success = false });
        }

        /// <summary>
        /// Fields mapping
        /// </summary>
        /// <param name="viewModelId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FieldsMapping([Required] Guid viewModelId)
        {
            var viewModel = await _pagesContext.ViewModels
                .Include(x => x.TableModel)
                .ThenInclude(x => x.TableFields)
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.TableModelFields)
                .FirstOrDefaultAsync(x => x.Id == viewModelId);
            if (viewModel == null) return NotFound();

            var baseProps = BaseModel.GetPropsName();
            var tableFields = viewModel.TableModel.TableFields.Where(x => !baseProps.Contains(x.Name)).ToList();
            ViewBag.ViewModel = viewModel;
            ViewBag.TableFields = tableFields;
            ViewBag.BaseProps = baseProps;
            return View();
        }

        /// <summary>
        /// Save new mapping
        /// </summary>
        /// <param name="viewModelFieldId"></param>
        /// <param name="tableFieldId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> FieldsMapping([Required] Guid viewModelFieldId, Guid tableFieldId)
        {
            var result = new ResultModel();
            var model = await _pagesContext.ViewModelFields.FirstOrDefaultAsync(x => x.Id == viewModelFieldId);
            if (model == null) return Json(result);
            model.TableModelFieldsId = tableFieldId;
            try
            {
                _pagesContext.Update(model);
                await _pagesContext.SaveChangesAsync();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return Json(result);
        }

        /// <summary>
        /// Update translated text for view model field
        /// </summary>
        /// <param name="fieldId"></param>
        /// <param name="translatedKey"></param>
        /// <returns></returns>
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        [Route("api/[controller]/[action]")]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> ChangeViewModelFieldTranslateText([Required] Guid fieldId,
            [Required] string translatedKey)
        {
            var response = new ResultModel();
            var field = await _pagesContext.ViewModelFields.FirstOrDefaultAsync(x => x.Id == fieldId);
            if (field == null)
            {
                response.Errors.Add(new ErrorModel("not_found", "Field not found!"));
                return Json(response);
            }

            field.Translate = translatedKey;
            _pagesContext.ViewModelFields.Update(field);
            try
            {
                await _pagesContext.SaveChangesAsync();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                response.Errors.Add(new ErrorModel("throw", e.Message));
            }

            return Json(response);
        }

        /// <summary>
        /// Set many to many configurations
        /// </summary>
        /// <param name="referenceEntity"></param>
        /// <param name="storageEntity"></param>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public virtual async Task<JsonResult> SaveManyToManyConfigurations(Guid? referenceEntity, Guid? storageEntity, Guid? fieldId)
        {
            var rs = new ResultModel();
            if (referenceEntity == null || storageEntity == null || fieldId == null)
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Invalid parameters, all are required!"));
                return Json(rs);
            }

            var field = await _pagesContext.ViewModelFields
                .Include(x => x.ViewModel)
                .ThenInclude(x => x.TableModel)
                .Include(x => x.Configurations)
                .Include(x => x.TableModelFields)
                .FirstOrDefaultAsync(x => x.Id.Equals(fieldId));

            if (field == null)
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Invalid data"));
                return Json(rs);
            }

            var viewModelTable = field.ViewModel?.TableModel;
            if (field.TableModelFields != null)
            {
                rs.Errors.Add(new ErrorModel("error", "This viewmodel field can't be used on a many to many relation, because he has a reference, remove reference and try again!"));
                return Json(rs);
            }

            var refEntity = await _pagesContext.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .FirstOrDefaultAsync(x => x.Id.Equals(referenceEntity));

            var stEntity = await _pagesContext.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefaultAsync(x => x.Id.Equals(storageEntity));

            if (refEntity == null || stEntity == null)
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Invalid data"));
                return Json(rs);
            }

            string propertyName = null;
            string refPropertyName = null;

            foreach (var tableField in stEntity.TableFields)
            {
                if (tableField.DataType != TableFieldDataType.Guid) continue;
                var configs = tableField.TableFieldConfigValues;
                var table = configs.FirstOrDefault(x =>
                    x.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable);
                var schema = configs.FirstOrDefault(x =>
                    x.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingSchemaTable);
                if (table == null || schema == null) continue;
                if (table.Value == refEntity.Name)
                {
                    refPropertyName = tableField.Name;
                }

                if (table.Value == viewModelTable?.Name)
                {
                    propertyName = tableField.Name;
                }
            }

            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(refPropertyName))
            {
                rs.Errors.Add(new ErrorModel(nameof(Nullable<Guid>), "Incompatible choose entities!"));
                return Json(rs);
            }

            field.VirtualDataType = ViewModelVirtualDataType.ManyToMany;

            field.Configurations = new List<ViewModelFieldConfiguration>
            {
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyReferenceEntityName,
                    ViewModelField = field,
                    Value = refEntity.Name
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyReferenceEntitySchema,
                    ViewModelField = field,
                    Value = refEntity.EntityType
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyStorageEntityName,
                    ViewModelField = field,
                    Value = stEntity.Name
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyStorageEntitySchema,
                    ViewModelField = field,
                    Value = stEntity.EntityType
                },

                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyReferencePropertyName,
                    ViewModelField = field,
                    Value = propertyName
                },
                new ViewModelFieldConfiguration
                {
                    ViewModelFieldCodeId = ViewModelConfigCode.MayToManyStorageSenderPropertyName,
                    ViewModelField = field,
                    Value = refPropertyName
                },
            };

            _pagesContext.ViewModelFields.Update(field);
            var rdb = await _pagesContext.PushAsync();
            if (rdb.IsSuccess)
            {
                rs.IsSuccess = true;
            }
            else
            {
                rs.Errors = rdb.Errors;
            }

            return Json(rs);
        }
    }
}