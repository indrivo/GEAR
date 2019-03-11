using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.Models;
using ST.Entities.Data;
using ST.Entities.Models.Nomenclator;
using ST.Entities.Models.Pages;
using ST.Entities.Services.Abstraction;

namespace ST.CORE.Controllers.Entity
{
	public class NomenclatorController : Controller
	{
		/// <summary>
		/// Context
		/// </summary>
		private readonly EntitiesDbContext _context;

		/// <summary>
		/// Inject Data Service
		/// </summary>
		private readonly IDynamicEntityDataService _dataService;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="dataService"></param>
		public NomenclatorController(EntitiesDbContext context, IDynamicEntityDataService dataService)
		{
			_context = context;
			_dataService = dataService;
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
		public async Task<IActionResult> Create(Nomenclator model)
		{
			if (model != null)
			{
				var req = await _dataService.AddSystem(model);
				if (req.IsSuccess)
					return RedirectToAction("Index");
				ModelState.AddModelError(string.Empty, "Fail to save!");
			}

			return View(model);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			if (id.Equals(Guid.Empty)) return NotFound();
			var model = await _dataService.GetByIdSystem<Nomenclator, Nomenclator>(id);

			if (!model.IsSuccess) return NotFound();

			return View(model.Result);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Edit(Nomenclator model)
		{
			if (model == null) return NotFound();
			var dataModel = (await _dataService.GetByIdSystem<Nomenclator, Nomenclator>(model.Id)).Result;

			if (dataModel == null) return NotFound();

			dataModel.Name = model.Name;
			dataModel.Description = model.Description;
			dataModel.Author = model.Author;
			dataModel.Changed = DateTime.Now;
			var req = await _dataService.UpdateSystem(dataModel);
			if (req.IsSuccess) return RedirectToAction("Index");
			ModelState.AddModelError(string.Empty, "Fail to save");
			return View(model);
		}

		/// <summary>
		/// Get Nomenclator items
		/// </summary>
		/// <param name="NomenclatorId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> GetNomenclator(Guid nomenclatorId, Guid? parentId = null)
		{
			ViewBag.NomenclatorId = nomenclatorId;
			ViewBag.ParentId = parentId;
			ViewBag.Nomenclator = (await _dataService.GetByIdSystem<Nomenclator, Nomenclator>(nomenclatorId)).Result;
			ViewBag.Parent = (parentId != null) ?
									(await _dataService.GetByIdSystem<NomenclatorItem, NomenclatorItem>(parentId.Value)).Result
									: null;
			return View();
		}

		/// <summary>
		/// Create Nomenclator item
		/// </summary>
		/// <param name="NomenclatorId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CreateItem(Guid nomenclatorId, Guid? parentId = null)
		{
			ViewBag.NomenclatorId = nomenclatorId;
			ViewBag.ParentId = parentId;
			ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
			return View();
		}

		/// <summary>
		/// Create Nomenclator item
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> CreateItem(NomenclatorItem model)
		{
			ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
			if (model != null)
			{
				//		model.AllowedRoles = "Administrator#";
				var req = await _dataService.AddSystem(model);
				if (req.IsSuccess)
					return RedirectToAction("GetNomenclator", new
					{
						model.NomenclatorId,
						ParentId = model.ParentId
					});
				ModelState.AddModelError(string.Empty, "Fail to save!");
			}

			return View(model);
		}

		/// <summary>
		/// Edit item
		/// </summary>
		/// <param name="itemId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> EditItem(Guid itemId)
		{
			ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
			var item = await _dataService.GetByIdSystem<NomenclatorItem, NomenclatorItem>(itemId);
			if (!item.IsSuccess) return NotFound();
			return View(item.Result);
		}

		/// <summary>
		/// Edit item
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditItem(NomenclatorItem model)
		{
			var rq = await _dataService.UpdateSystem(model);
			if (rq.IsSuccess)
			{
				return RedirectToAction("GetNomenclator", new
				{
					model.NomenclatorId,
					//		ParentId = model.ParentNomenclatorItemId
				});
			}

			ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
			ModelState.AddModelError(string.Empty, "Fail to save!");
			return View(model);
		}

		/// <summary>
		/// Load Nomenclator items
		/// </summary>
		/// <param name="param"></param>
		/// <param name="NomenclatorId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> LoadNomenclatorItems(DTParameters param, Guid nomenclatorId, Guid? parentId = null)
		{
			
			var filtered = await _dataService.Filter<NomenclatorItem>(param.Search.Value, param.SortOrder, param.Start,
				param.Length, x => x.NomenclatorId.Equals(nomenclatorId) && x.ParentId.Equals(parentId));

			var finalResult = new DTResult<NomenclatorItem>
			{
				draw = param.Draw,
				data = filtered.Item1,
				recordsFiltered = filtered.Item2,
				recordsTotal = filtered.Item1.Count()
			};
			return Json(finalResult);
		}

		/// <summary>
		/// Load page types with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public async Task<JsonResult> LoadPages(DTParameters param)
		{
			var filtered = await _dataService.Filter<Nomenclator>(param.Search.Value, param.SortOrder, param.Start,
				param.Length);

			var finalResult = new DTResult<Nomenclator>
			{
				draw = param.Draw,
				data = filtered.Item1,
				recordsFiltered = filtered.Item2,
				recordsTotal = filtered.Item1.Count()
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
		public async Task<JsonResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete Nomenclator!", success = false });
			var Nomenclator = await _dataService.DeletePermanent<Nomenclator>(Guid.Parse(id));
			if (!Nomenclator.IsSuccess) return Json(new { message = "Fail to delete Nomenclator!", success = false });

			return Json(new { message = "Nomenclator was delete with success!", success = true });
		}


		/// <summary>
		/// Delete page type by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Route("api/[controller]/[action]")]
		[ValidateAntiForgeryToken]
		[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		public async Task<JsonResult> DeleteNomenclatorItem(string id)
		{
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete Nomenclator item!", success = false });
			var Nomenclator = await _dataService.DeletePermanent<NomenclatorItem>(Guid.Parse(id));
			if (!Nomenclator.IsSuccess) return Json(new { message = "Fail to delete Nomenclator item!", success = false });

			return Json(new { message = "Model was delete with success!", success = true });
		}
	}
}