using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.Entities.Data;
using ST.Entities.Models.Forms;
using ST.Entities.Models.Tables;
using ST.Entities.Services.Abstraction;
using ST.Entities.ViewModels.Form;
using ST.Identity.Attributes;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Configuration.Models;
using ST.CORE.ViewModels.FormsViewModels;
using ST.DynamicEntityStorage;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Extensions;
using ST.Identity.Data;
using ST.Identity.Services.Abstractions;
using ST.MultiTenant.Helpers;
using ST.MultiTenant.Services.Abstractions;
using ST.Notifications.Abstractions;
using ST.Shared;
using ST.Shared.Attributes;
using Settings = ST.Configuration.Settings;

namespace ST.CORE.Controllers.Entity
{
	/// <inheritdoc />
	/// <summary>
	/// Forms manipulation
	/// </summary>
	[Authorize]
	public class FormController : BaseController
	{
		#region Inject
		private IFormService FormService { get; }

		/// <summary>
		/// Inject re
		/// </summary>
		private IBaseBusinessRepository<EntitiesDbContext> Repository { get; }

		/// <summary>
		/// Inject dynamic service
		/// </summary>
		private readonly IDynamicService _service;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="applicationDbContext"></param>
		/// <param name="userManager"></param>
		/// <param name="roleManager"></param>
		/// <param name="notify"></param>
		/// <param name="organizationService"></param>
		/// <param name="formService"></param>
		/// <param name="cacheService"></param>
		/// <param name="repository"></param>
		/// <param name="service"></param>
		public FormController(EntitiesDbContext context, ApplicationDbContext applicationDbContext,
			UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
			INotify<ApplicationRole> notify, IOrganizationService organizationService, IFormService formService,
			ICacheService cacheService,
			IBaseBusinessRepository<EntitiesDbContext> repository, IDynamicService service)
			: base(context, applicationDbContext, userManager, roleManager, notify, organizationService, cacheService)
		{
			FormService = formService;
			Repository = repository;
			_service = service;
		}

		/// <summary>
		/// Create new form
		/// </summary>
		/// <returns></returns>
		public IActionResult Create()
		{
			ViewData["models"] = Repository.GetAll<TableModel>(x => x.IsDeleted == false);
			ViewData["formTypes"] = Repository.GetAll<FormType>(x => x.IsDeleted == false).OrderBy(s => s.Code);
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
			ViewBag.FormType = Repository.GetSingle<FormType>(formType);
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
			var form = Context.Forms
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
			var bdForm = Context.Forms.FirstOrDefault(x => x.Id.Equals(formId));
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
			ViewBag.Form = Context.Forms.FirstOrDefault(x => x.Id == formId);
			ViewBag.FormType = FormService.GetTypeByFormId(formId);
			return View();
		}


		[HttpGet]
		public JsonResult GetFormTableReference(Guid? formId)
		{
			if (formId == null) return default;
			var table = Context.Forms.Include(x => x.Table).FirstOrDefault(x => x.Id == formId)?.Table;
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
			var filtered = Context.Filter<Form>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount, x => (entityId != Guid.Empty && x.TableId == entityId) || entityId == Guid.Empty);


			var finalResult = new DTResult<FormListViewModel>
			{
				draw = param.Draw,
				data = filtered.Select(x => new FormListViewModel
				{
					Id = x.Id,
					Name = x.Name,
					Created = x.Created,
					TableName = Context.Table.FirstOrDefault(o => o.Id == x.TableId)?.Name,
					IsDeleted = x.IsDeleted,
					TypeId = x.TypeId,
					Type = x.Type,
					Description = x.Description,
					Author = UserManager.Users.FirstOrDefault(y => y.Id.Equals(x.Author))?.Name,
					Changed = x.Changed,
					Table = x.Table,
					ModifiedBy = x.ModifiedBy,
					TableId = x.TableId
				}).ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count
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
		[Authorize(Roles = Settings.SuperAdmin)]
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
		[Authorize(Roles = Settings.SuperAdmin)]
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
		[Authorize(Roles = Settings.SuperAdmin)]
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
			var form = await Context.Forms
				.Include(x => x.Table)
				.ThenInclude(x => x.TableFields)
				.Include(x => x.Fields)
				.ThenInclude(x => x.Attrs)
				.FirstOrDefaultAsync(x => x.Id == formId);

			if (form == null)
			{
				result.Errors.Add(new ErrorModel(string.Empty, "Form not found"));
				return Json(result);
			}
			var obj = await _service.Table(form.Table.Name).GetById<object>(itemId);
			if (!obj.IsSuccess)
			{
				result.Errors.Add(new ErrorModel(string.Empty, "Object not found"));
				return Json(result);
			}
			var objDict = ObjectService.GetDictionary(obj.Result);

			var formValues = FormService.GetValuesForEditForm(form, objDict);
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
			var form = await Context.Forms
				.Include(x => x.Table)
				.Include(x => x.Fields)
				.ThenInclude(x => x.TableField)
				.Include(x => x.Fields)
				.ThenInclude(x => x.Attrs)
				.Include(x => x.Fields)
				.ThenInclude(x => x.Config)
				.Include(x => x.Fields)
				.ThenInclude(x => x.Meta)
				.FirstOrDefaultAsync(x => x.Id == formId);
			if (form == null) return NotFound();
			return View(form);
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
			var field = await Context.Fields
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
		private StCollection<FormValidation> SystemFieldValidations =>
			CacheService
				.Get<StCollection<FormValidation>>("_fieldValidations").GetAwaiter().GetResult()
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

			var field = await Context.Fields
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
						Context.Attrs.Update(attr);
					}
					else
					{
						Context.Attrs.Remove(item);
					}
				}
			}

			foreach (var item in selectedValidations)
			{
				await Context.Attrs.AddAsync(new Attrs
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
		private async Task<StCollection<FormValidation>> GetOrUpdateForm()
		{
			var systemValidations = JsonParser
				.ReadArrayDataFromJsonFile<StCollection<FormValidation>>(
					Path.Combine(AppContext.BaseDirectory,
					"FormValidations.json"));

			if (systemValidations != null)
			{
				await CacheService
					.Set("_fieldValidations", systemValidations);
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
		[Authorize(Roles = Settings.SuperAdmin)]
		public JsonResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete form!", success = false });
			var res = FormService.DeleteForm(Guid.Parse(id));
			return Json(new { message = "Form was delete with success!", success = res.IsSuccess });
		}
	}
}