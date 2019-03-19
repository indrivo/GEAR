using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.Models;
using ST.CORE.ViewModels.Pages;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Pages;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Services.Abstractions;
using ST.Notifications.Abstraction;
using ST.Procesess.Data;

namespace ST.CORE.Controllers.Entity
{
	public class BlocksController : BaseController
	{
		public BlocksController(EntitiesDbContext context, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, INotify notify, IOrganizationService organizationService, ProcessesDbContext processesDbContext) : base(context, applicationDbContext, userManager, roleManager, notify, organizationService, processesDbContext)
		{
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
		/// Create new block view
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Create()
		{
			var model = new CreateBlockViewModel
			{
				BlockCategories = Context.BlockCategories.ToList()
			};
			return View(model);
		}

		/// <summary>
		/// Add new block
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult Create([Required]CreateBlockViewModel model)
		{
			try
			{
				model.TenantId = CurrentUserTenantId;
				model.Author = GetCurrentUser().Id;
				model.Changed = DateTime.Now;
				Context.Blocks.Add(model);
				Context.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			model.BlockCategories = Context.BlockCategories.ToList();
			return View(model);
		}

		/// <summary>
		/// Edit block
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Edit(Guid? id)
		{
			if (id == null) return NotFound();
			var req = Context.Blocks.Include(x => x.BlockCategory).Single(x => x.Id == id);
			if (req == null) return NotFound();
			var model = req.Adapt<CreateBlockViewModel>();
			model.BlockCategories = Context.BlockCategories.ToList();
			return View(model);
		}

		/// <summary>
		/// Edit block post action
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult Edit([Required] CreateBlockViewModel model)
		{
			model.Changed = DateTime.Now;
			try
			{
				Context.Blocks.Update(model);
				Context.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
				Console.WriteLine(e);
			}
			return View(model);
		}

		/// <summary>
		/// Load blocks with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public JsonResult LoadPages(DTParameters param)
		{
			var filtered = Context.Filter<Block>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount);

			var finalResult = new DTResult<Block>
			{
				draw = param.Draw,
				data = filtered.ToList(),
				recordsFiltered = totalCount,
				recordsTotal = filtered.Count()
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
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete block!", success = false });
			var page = Context.Blocks.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (page == null) return Json(new { message = "Fail to delete block!", success = false });

			try
			{
				Context.Blocks.Remove(page);
				Context.SaveChanges();
				return Json(new { message = "Block was delete with success!", success = true });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return Json(new { message = "Fail to delete block!", success = false });
		}
	}
}
