using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.Configuration;
using ST.Configuration.Models;
using ST.CORE.Attributes;
using ST.CORE.ViewModels;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Entities.Models.RenderTemplates;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Data;
using ST.Identity.Services.Abstractions;
using ST.MultiTenant.Services.Abstractions;
using ST.Notifications.Abstraction;
using ST.Procesess.Data;

namespace ST.CORE.Controllers.Entity
{
	[Authorize(Roles = Settings.SuperAdmin)]
	public class TemplatesController : BaseController
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="applicationDbContext"></param>
		/// <param name="userManager"></param>
		/// <param name="roleManager"></param>
		/// <param name="notify"></param>
		/// <param name="organizationService"></param>
		/// <param name="processesDbContext"></param>
		/// <param name="cacheService"></param>
		public TemplatesController(EntitiesDbContext context, ApplicationDbContext applicationDbContext,
			UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, INotify<ApplicationRole> notify,
			IOrganizationService organizationService, ProcessesDbContext processesDbContext, ICacheService cacheService) : base(context, applicationDbContext, userManager, roleManager, notify, organizationService, processesDbContext, cacheService)
		{
		}

		/// <summary>
		/// Load page types with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public JsonResult LoadPages(DTParameters param)
		{
			var filtered = Context.Filter<Template>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount);

			var finalResult = new DTResult<Template>
			{
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count
			};
			return Json(finalResult);
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
		public async Task<IActionResult> Create([Required]Template model)
		{
			if (Context.Templates.Any(x => x.Name == model.Name))
			{
				ModelState.AddModelError(string.Empty, "Name is used by another template!");
				return View(model);
			}
			try
			{
				model.IdentifierName = $"template_{model.Name}";
				model.TenantId = CurrentUserTenantId;
				model.Author = GetCurrentUser()?.Id;
				Context.Templates.Add(model);
				Context.SaveChanges();
				await CacheService.Set(model.IdentifierName, new TemplateCacheModel
				{
					Identifier = model.IdentifierName,
					Value = model.Value
				});
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}

			return View(model);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Edit(Guid id)
		{
			if (id.Equals(Guid.Empty)) return NotFound();
			var model = Context.Templates.FirstOrDefault(x => x.Id.Equals(id));
			if (model == null) return NotFound();

			return View(model);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Edit(Template model)
		{
			if (model == null) return NotFound();
			var dataModel = Context.Templates.FirstOrDefault(x => x.Id.Equals(model.Id));

			if (dataModel == null) return NotFound();

			dataModel.Name = model.Name;
			dataModel.Description = model.Description;
			dataModel.Author = model.Author;
			dataModel.Value = model.Value;
			dataModel.Changed = DateTime.Now;
			dataModel.ModifiedBy = GetCurrentUser()?.Id;
			try
			{
				Context.Templates.Update(dataModel);
				Context.SaveChanges();
				await CacheService.Set(dataModel.IdentifierName, new TemplateCacheModel
				{
					Identifier = model.IdentifierName,
					Value = dataModel.Value
				});
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}

			return View(model);
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
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete template!", success = false });
			var template = Context.Templates.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (template == null) return Json(new { message = "Fail to delete template!", success = false });

			try
			{
				Context.Templates.Remove(template);
				Context.SaveChanges();
				await CacheService.RemoveAsync(template.IdentifierName);
				return Json(new { message = "Template was delete with success!", success = true });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return Json(new { message = "Fail to delete template!", success = false });
		}


		/// <summary>
		/// Get template by identifier
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetTemplateByIdentifier(string identifier)
		{
			var result = new ResultModel<string>();
			if (string.IsNullOrEmpty(identifier))
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel("null", "Not specified identifier")
				};
				return Json(result);
			}

			var template = await CacheService.Get<TemplateCacheModel>(identifier);
			if (template == null)
			{
				var templateFromStore = Context.Templates.FirstOrDefault(x => x.IdentifierName == identifier);
				if (templateFromStore == null)
				{
					result.Errors = new List<IErrorModel>
					{
						new ErrorModel("null", "Template not found!")
					};
					return Json(result);
				}
				await CacheService.Set(templateFromStore.IdentifierName, new TemplateCacheModel
				{
					Identifier = templateFromStore.IdentifierName,
					Value = templateFromStore.Value
				});
				result.IsSuccess = true;
				result.Result = templateFromStore.Value;
			}
			else
			{
				result.IsSuccess = true;
				result.Result = template.Value;
			}

			return Json(result);
		}
	}
}
