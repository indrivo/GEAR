using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.Models;
using ST.CORE.ViewModels.Pages;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Pages;

namespace ST.CORE.Controllers.Entity
{
	public class BlocksController : Controller
	{
		#region Inject
		/// <summary>
		/// Inject db context
		/// </summary>
		private readonly EntitiesDbContext _context;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		public BlocksController(EntitiesDbContext context)
		{
			_context = context;
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
				BlockCategories = _context.BlockCategories.ToList()
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
				_context.Blocks.Add(model);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			model.BlockCategories = _context.BlockCategories.ToList();
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
			var req = _context.Blocks.Include(x => x.BlockCategory).Single(x => x.Id == id);
			if (req == null) return NotFound();
			var model = req.Adapt<CreateBlockViewModel>();
			model.BlockCategories = _context.BlockCategories.ToList();
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
				_context.Blocks.Update(model);
				_context.SaveChanges();
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
			var filtered = _context.Filter<Block>(param.Search.Value, param.SortOrder, param.Start,
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
			var page = _context.Blocks.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (page == null) return Json(new { message = "Fail to delete block!", success = false });

			try
			{
				_context.Blocks.Remove(page);
				_context.SaveChanges();
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
