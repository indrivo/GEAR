using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.ViewModels;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Pages;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Data;
using ST.MultiTenant.Services.Abstractions;
using ST.Notifications.Abstraction;
using ST.Procesess.Data;

namespace ST.CORE.Controllers.Entity
{
	public class BlockCategoryController : BaseController
	{
		public BlockCategoryController(EntitiesDbContext context, ApplicationDbContext applicationDbContext,
			UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, INotify<ApplicationRole> notify,
			IOrganizationService organizationService, ProcessesDbContext processesDbContext) : base(context, applicationDbContext, userManager, roleManager, notify, organizationService, processesDbContext)
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
			var filtered = Context.Filter<BlockCategory>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount);

			var finalResult = new DTResult<BlockCategory>
			{
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count()
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
		public IActionResult Create([Required]BlockCategory model)
		{
			try
			{
				Context.BlockCategories.Add(model);
				Context.SaveChanges();

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
			var model = Context.BlockCategories.FirstOrDefault(x => x.Id.Equals(id));
			if (model == null) return NotFound();

			return View(model);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult Edit(BlockCategory model)
		{
			if (model == null) return NotFound();
			var dataModel = Context.BlockCategories.FirstOrDefault(x => x.Id.Equals(model.Id));

			if (dataModel == null) return NotFound();

			dataModel.Name = model.Name;
			dataModel.Description = model.Description;
			dataModel.Author = model.Author;
			dataModel.Changed = DateTime.Now;
			try
			{
				Context.BlockCategories.Update(dataModel);
				Context.SaveChanges();
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
		public JsonResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete block category!", success = false });
			var page = Context.BlockCategories.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (page == null) return Json(new { message = "Fail to delete block category!", success = false });

			try
			{
				Context.BlockCategories.Remove(page);
				Context.SaveChanges();
				return Json(new { message = "Block category was delete with success!", success = true });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return Json(new { message = "Fail to delete block category!", success = false });
		}
	}
}
