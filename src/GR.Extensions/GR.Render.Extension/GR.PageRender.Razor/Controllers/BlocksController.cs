using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Cache.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Data;
using GR.Identity.Data;
using GR.Notifications.Abstractions;
using GR.PageRender.Razor.ViewModels.PageViewModels;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.BaseControllers;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;

namespace GR.PageRender.Razor.Controllers
{
	[Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
	public class BlocksController : BaseController<ApplicationDbContext, EntitiesDbContext, ApplicationUser, ApplicationRole, Tenant, INotify<ApplicationRole>>
    {
        private readonly IDynamicPagesContext _pagesContext;

        public BlocksController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify, IDynamicPagesContext pagesContext) : base(userManager, roleManager, cacheService, applicationDbContext, context, notify)
        {
            _pagesContext = pagesContext;
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
				BlockCategories = _pagesContext.BlockCategories.ToList()
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
                _pagesContext.Blocks.Add(model);
                _pagesContext.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			model.BlockCategories = _pagesContext.BlockCategories.ToList();
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
			var req = _pagesContext.Blocks.Include(x => x.BlockCategory).Single(x => x.Id == id);
			if (req == null) return NotFound();
			var model = req.Adapt<CreateBlockViewModel>();
			model.BlockCategories = _pagesContext.BlockCategories.ToList();
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
                _pagesContext.Blocks.Update(model);
                _pagesContext.SaveChanges();
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
			var filtered = _pagesContext.FilterAbstractContext<Block>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount);

			var finalResult = new DTResult<Block>
			{
				Draw = param.Draw,
				Data = filtered.ToList(),
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
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete block!", success = false });
			var page = _pagesContext.Blocks.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (page == null) return Json(new { message = "Fail to delete block!", success = false });

			try
			{
                _pagesContext.Blocks.Remove(page);
                _pagesContext.SaveChanges();
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
