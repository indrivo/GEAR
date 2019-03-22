using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.ViewModels;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Pages;

namespace ST.CORE.Controllers.Entity
{
	public class PageTypeController : Controller
	{
		/// <summary>
		/// Context
		/// </summary>
		private readonly EntitiesDbContext _context;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		public PageTypeController(EntitiesDbContext context)
		{
			_context = context;
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
		public IActionResult Create(PageType model)
		{
			if (model != null)
			{
				try
				{
					_context.PageTypes.Add(model);
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
				catch (Exception e)
				{
					ModelState.AddModelError(string.Empty, e.Message);
				}
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
			var model = _context.PageTypes.FirstOrDefault(x => x.Id.Equals(id));
			if (model == null) return NotFound();

			return View(model);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult Edit(PageType model)
		{
			if (model == null) return NotFound();
			var dataModel = _context.PageTypes.FirstOrDefault(x => x.Id.Equals(model.Id));

			if (dataModel == null) return NotFound();

			dataModel.Name = model.Name;
			dataModel.Description = model.Description;
			dataModel.Author = model.Author;
			dataModel.Changed = DateTime.Now;
			try
			{
				_context.PageTypes.Update(dataModel);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}

			return View(model);
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
			var filtered = _context.Filter<PageType>(param.Search.Value, param.SortOrder, param.Start,
				param.Length,
				out var totalCount);

			var finalResult = new DTResult<PageType>
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
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete page type!", success = false });
			var page = _context.PageTypes.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
			if (page == null) return Json(new { message = "Fail to delete page type!", success = false });

			try
			{
				_context.PageTypes.Remove(page);
				_context.SaveChanges();
				return Json(new { message = "Page type  was delete with success!", success = true });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			return Json(new { message = "Fail to delete page type!", success = false });
		}
	}
}