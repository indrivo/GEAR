using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using GR.Cache.Abstractions;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Abstractions.Models;
using GR.Forms.Razor.ViewModels.FormsViewModels;
using GR.Identity.Data;
using GR.Identity.Data.Permissions;
using GR.Notifications.Abstractions;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.BaseControllers;
using GR.Core.Helpers;
using GR.Entities.Abstractions;
using GR.Entities.Data;
using GR.Forms.Abstractions;
using GR.Forms.Abstractions.Models.FormModels;
using GR.Forms.Abstractions.ViewModels.FormViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Permissions.Abstractions.Attributes;

namespace GR.Forms.Razor.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Forms manipulation
    /// </summary>
    public class FormController : BaseIdentityController<ApplicationDbContext, EntitiesDbContext, GearUser, GearRole, Tenant, INotify<GearRole>>
    {
        #region Inject

        /// <summary>
        /// Inject form service
        /// </summary>
        private IFormService FormService { get; }

        /// <summary>
        /// Inject form context
        /// </summary>
        private readonly IFormContext _formContext;

        /// <summary>
        /// Inject entity db context
        /// </summary>
        private readonly IEntityContext _entityContext;

        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IDynamicService _service;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;
        #endregion


        public FormController(UserManager<GearUser> userManager, RoleManager<GearRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<GearRole> notify, IDynamicService service, IFormService formService, IFormContext formContext, IEntityContext entityContext) : base(userManager, roleManager, applicationDbContext, context, notify)
        {
            _cacheService = cacheService;
            _service = service;
            FormService = formService;
            _formContext = formContext;
            _entityContext = entityContext;
        }


        /// <summary>
        /// Create new form
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewData["models"] = Context.Table.Where(x => !x.IsDeleted).ToList();
            ViewData["formTypes"] = _formContext.FormTypes.Where(x => !x.IsDeleted).ToList().OrderBy(s => s.Code);
            return View();
        }

        /// <summary>
        /// Preview form by model
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="tableId"></param>
        /// <param name="formType"></param>
        /// <returns></returns>
        public IActionResult Generate(string mode, Guid tableId, Guid formType)
        {
            ViewBag.FormType = _formContext.FormTypes.FirstOrDefault(x => x.Id == formType);
            return View();
        }

        /// <summary>
        /// Get form for update
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(Guid formId)
        {
            var form = _formContext.Forms
                .Include(x => x.Type)
                .FirstOrDefault(x => x.Id.Equals(formId));
            if (form == null) return NotFound();
            ViewBag.Form = form;

            return View();
        }

        /// <summary>
        /// Create new form
        /// </summary>
        /// <param name="form"></param>
        /// <param name="formId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="postUrl"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult UpdateForm(FormViewModel form, [Required]Guid formId,
            string name, string description, string postUrl, string redirectUrl)
        {
            var bdForm = _formContext.Forms.FirstOrDefault(x => x.Id.Equals(formId));
            if (bdForm == null) return Json(new ResultModel());
            var res = FormService.DeleteForm(formId);
            if (!res.IsSuccess) return Json(new ResultModel());
            var response = FormService.CreateForm(new FormCreateDetailsViewModel
            {
                Id = formId,
                Created = bdForm.Created,
                Author = bdForm.Author,
                Description = description,
                ModifiedBy = GetCurrentUser()?.Id,
                TenantId = CurrentUserTenantId,
                Name = name,
                PostUrl = postUrl,
                RedirectUrl = redirectUrl,
                Model = form,
                TableId = bdForm.TableId,
                FormTypeId = bdForm.TypeId,
                EditMode = true
            });

            return Json(response);
        }

        /// <summary>
        /// Get Form by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult GetForm(Guid id)
        {
            var response = FormService.GetFormById(id);
            return Json(response);
        }

        /// <summary>
        /// Get table fields for preview form
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult GetTableFields(Guid tableId)
        {
            return FormService.GetTableFields(tableId);
        }

        /// <summary>
        /// Get by page
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmFormRead)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Preview form
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        public IActionResult Preview(Guid formId)
        {
            ViewBag.FormId = formId;
            ViewBag.Form = _formContext.Forms.FirstOrDefault(x => x.Id == formId);
            ViewBag.FormType = FormService.GetTypeByFormId(formId);
            return View();
        }


        [HttpGet]
        public JsonResult GetFormTableReference(Guid? formId)
        {
            if (formId == null) return default;
            var form = _formContext.Forms.FirstOrDefault(x => x.Id == formId);
            if (form == null) return Json(default(TableModel));
            var table = Context.Table.FirstOrDefault(x => x.Id == form.TableId);
            return Json(table);
        }

        /// <summary>
        /// Load forms with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadForms(DTParameters param, Guid entityId)
        {
            var filtered = _formContext.FilterAbstractContext<Form>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount, x => entityId != Guid.Empty && x.TableId == entityId || entityId == Guid.Empty);


            var finalResult = new DTResult<FormListViewModel>
            {
                Draw = param.Draw,
                Data = filtered.Select(x => new FormListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Created = x.Created,
                    TableName = Context.Table.FirstOrDefault(o => o.Id == x.TableId)?.Name,
                    IsDeleted = x.IsDeleted,
                    TypeId = x.TypeId,
                    Type = x.Type,
                    Description = x.Description,
                    Author = x.Author,
                    Changed = x.Changed,
                    ModifiedBy = x.ModifiedBy,
                    TableId = x.TableId
                }).ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(finalResult);
        }

        /// <summary>
        /// Create new form
        /// </summary>
        /// <param name="form"></param>
        /// <param name="tableId"></param>
        /// <param name="formTypeId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="postUrl"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> CreateNewForm(FormViewModel form, Guid tableId, Guid formTypeId,
            string name, string description, string postUrl, string redirectUrl)
        {
            var user = await GetCurrentUserAsync();
            var response = FormService.CreateForm(new FormCreateDetailsViewModel
            {
                Description = description,
                Name = name,
                PostUrl = postUrl,
                Author = user.Id,
                ModifiedBy = user.Id,
                RedirectUrl = redirectUrl,
                Model = form,
                TableId = tableId,
                FormTypeId = formTypeId
            });
            return Json(response);
        }

        /// <summary>
        /// Get entity fields
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public JsonResult GetEntityFields(Guid tableId)
        {
            return FormService.GetEntityFields(tableId);
        }

        /// <summary>
        /// Get entity reference fields
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entitySchema"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public JsonResult GetEntityReferenceFields(string entityName, string entitySchema)
        {
            return FormService.GetEntityReferenceFields(entityName, entitySchema);
        }

        /// <summary>
        /// Get entity fields by entity id and entityFieldId
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityFieldId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public JsonResult GetReferenceFields(Guid? entityId, Guid? entityFieldId)
        {
            return FormService.GetReferenceFields(entityId, entityFieldId);
        }

        /// <summary>
        /// Get values for object on edit
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetValuesFormObjectEditInForm([Required]Guid formId, [Required] Guid itemId)
        {
            var result = new ResultModel
            {
                Errors = new List<IErrorModel>()
            };
            var form = await _formContext.Forms
                .Include(x => x.Fields)
                .ThenInclude(x => x.Attrs)
                .FirstOrDefaultAsync(x => x.Id == formId);

            if (form == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Form not found"));
                return Json(result);
            }

            var model = form.Adapt<FormFieldsViewModel>();
            model.Table = _entityContext.Table.Include(x => x.TableFields).FirstOrDefault(x => x.Id == formId);
            if (model.Table == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Form table reference not found"));
                return Json(result);
            }
            var obj = await _service.GetById(model.Table.Name, itemId);
            if (!obj.IsSuccess)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Object not found"));
                return Json(result);
            }

            var formValues = FormService.GetValuesForEditForm(form, obj.Result);
            if (!formValues.IsSuccess) return Json(result);
            result.Result = formValues.Result;
            result.IsSuccess = formValues.IsSuccess;
            return Json(result);
        }

        /// <summary>
        /// Get form fields
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFormFields([Required] Guid? formId)
        {
            if (formId == null) return NotFound();
            var form = await _formContext.Forms
                .Include(x => x.Fields)
                .ThenInclude(x => x.Attrs)
                .Include(x => x.Fields)
                .ThenInclude(x => x.Config)
                .Include(x => x.Fields)
                .ThenInclude(x => x.Meta)
                .FirstOrDefaultAsync(x => x.Id == formId);
            if (form == null) return NotFound();
            var table = await _entityContext.Table
                .Include(x => x.TableFields)
                .FirstOrDefaultAsync(x => x.Id == form.TableId);
            var model = form.Adapt<FormFieldsViewModel>();
            model.Table = table;
            model.Fields = model.Fields.Select(field =>
            {
                field.TableField = table.TableFields.FirstOrDefault(x => x.Id == field.TableFieldId);
                return field;
            }).ToList();
            
            return View(model);
        }

        /// <summary>
        /// Get field attributes for validations
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFieldAttributes([Required]Guid? fieldId)
        {
            if (fieldId == null) return NotFound();
            var field = await _formContext.Fields
                .Include(x => x.Form)
                .Include(x => x.Attrs)
                .FirstOrDefaultAsync(x => x.Id == fieldId);
            if (field == null) return NotFound();
            var systemValidations = SystemFieldValidations;
            var data = field.Attrs.Where(x => systemValidations.Select(u => u.Name).Contains(x.Key))
                .Select(x => new FormValidation
                {
                    Name = x.Key,
                    Code = systemValidations.FirstOrDefault(c => c.Name == x.Key)?.Code,
                    Default = x.Value,
                    Description = systemValidations.FirstOrDefault(c => c.Name == x.Key)?.Description,
                    IsSelected = true
                }).ToList();

            foreach (var item in systemValidations)
            {
                if (!data.Select(x => x.Code).Contains(item.Code))
                {
                    data.Add(item);
                }
            }
            var model = new FieldValidationViewModel
            {
                Field = field,
                FormValidations = data
            };
            return View(model);
        }

        /// <summary>
        /// Get system field validations
        /// </summary>
        private Collection<FormValidation> SystemFieldValidations =>
            _cacheService
                .GetAsync<Collection<FormValidation>>("_fieldValidations").GetAwaiter().GetResult()
            ?? GetOrUpdateForm().GetAwaiter().GetResult();

        /// <summary>
        /// Update validations for field
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetFieldAttributes([Required]FieldValidationViewModel model)
        {
            if (model.Field == null)
            {
                ModelState.AddModelError(string.Empty, "Field not found!");
                return View(model);
            }

            var field = await _formContext.Fields
                .Include(x => x.Attrs)
                .FirstOrDefaultAsync(x => x.Id == model.Field.Id);

            if (field == null)
            {
                ModelState.AddModelError(string.Empty, "Field not found!");
                return View(model);
            }

            var systemValidations = SystemFieldValidations.ToList();
            var selectedValidations = model.FormValidations.Where(x => x.IsSelected).ToList();
            //var nonSelectedValidations = model.FormValidations.Where(x => !x.IsSelected).ToList();
            foreach (var item in field.Attrs)
            {
                if (systemValidations.Select(x => x.Name).Contains(item.Key))
                {
                    var selected = selectedValidations.FirstOrDefault(x => x.Name == item.Key);
                    if (selected != null)
                    {
                        selectedValidations.Remove(selected);
                        if (item.Value == selected.Default) continue;
                        var attr = item;
                        attr.Value = selected.Default;
                        _formContext.Attrs.Update(attr);
                    }
                    else
                    {
                        _formContext.Attrs.Remove(item);
                    }
                }
            }

            foreach (var item in selectedValidations)
            {
                await _formContext.Attrs.AddAsync(new Attrs
                {
                    Field = field,
                    Value = item.Default,
                    Key = item.Name,
                    Type = AttrValueType.String,
                    TenantId = CurrentUserTenantId
                });
            }

            await Context.SaveChangesAsync();
            return RedirectToAction("GetFormFields", new { formId = model.Field.FormId });
        }

        /// <summary>
        /// Get or save to cache
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private async Task<Collection<FormValidation>> GetOrUpdateForm()
        {
            var systemValidations = JsonParser
                .ReadArrayDataFromJsonFile<Collection<FormValidation>>(
                    Path.Combine(AppContext.BaseDirectory,
                    "Configuration/FormValidations.json"));

            if (systemValidations != null)
            {
                await _cacheService
                    .SetAsync("_fieldValidations", systemValidations);
            }
            return systemValidations;
        }

        /// <summary>
        /// Delete form by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public JsonResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete form!", success = false });
            var res = FormService.DeleteForm(Guid.Parse(id));
            return Json(new { message = "Form was delete with success!", success = res.IsSuccess });
        }
    }
}