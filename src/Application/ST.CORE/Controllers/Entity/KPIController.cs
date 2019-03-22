using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ST.BaseBusinessRepository;
using ST.CORE.Attributes;
using ST.CORE.ViewModels;
using ST.CORE.ViewModels.KPIViewModels;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.KPI;
using ST.Entities.Models.Nomenclator;
using ST.Entities.Models.Pages;
using ST.Entities.Services.Abstraction;

namespace ST.CORE.Controllers.Entity
{
	public class KPIController : Controller
	{
		/// <summary>
		/// Context
		/// </summary>
		private readonly EntitiesDbContext _context;

		/// <summary>
		/// Inject Data Service
		/// </summary>
		private readonly IDynamicService _service;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="context"></param>
		/// <param name="service"></param>
		public KPIController(EntitiesDbContext context, IDynamicService service)
		{
			_context = context;
			_service = service;
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
		public async Task<IActionResult> Create()
		{
			var model = new UpdateKPIViewModel();
			var measurementsItemsCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Measurements Items Category");
			var viewMeasurementUnits = (measurementsItemsCategory.IsSuccess == true) ? await _service.Table("NomMeasurement").GetAll<dynamic>(x => x.NomenclatorId == measurementsItemsCategory.Result.FirstOrDefault().Id) : null;

			var calculationPeriodCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Calculation period Category");
			var viewCalculationPeriodUnits = (calculationPeriodCategory.IsSuccess == true) ? await _service.Table("NomPeriod").GetAll<dynamic>(x => x.NomenclatorId == calculationPeriodCategory.Result.FirstOrDefault().Id) : null;

			var kPICategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "KPI Category");
			var viewKPICategoryUnits = (kPICategory.IsSuccess == true) ? await _service.Table("NomKPICategory").GetAll<dynamic>(x => x.NomenclatorId == kPICategory.Result.FirstOrDefault().Id) : null;

			var selectedMeasurementItem = viewMeasurementUnits.Result.Where(x => x.Id == model.MeasurementUnitId).FirstOrDefault();
			var fulfillmentCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Criterion of fulfillment Category");
			var viewFulfillmentCategoryUnits = (selectedMeasurementItem?.Name == "Boolean") ? ((fulfillmentCategory.IsSuccess == true) ? await _service.Table("NomFulfillment").GetAll<dynamic>(x => x.NomenclatorId == fulfillmentCategory.Result.FirstOrDefault().Id & x.DependencyId == selectedMeasurementItem.Id) : null) : ((fulfillmentCategory.IsSuccess == true) ? await _service.Table("NomFulfillment").GetAll<dynamic>(x => x.NomenclatorId == fulfillmentCategory.Result.FirstOrDefault().Id) : null);

			model.MeasurementUnits = (viewMeasurementUnits.IsSuccess == true) ? viewMeasurementUnits.Result.To<object, IEnumerable<NomMeasurement>>() : null;
			model.NomPeriod = (viewCalculationPeriodUnits.IsSuccess == true) ? viewCalculationPeriodUnits.Result.To<object, IEnumerable<NomPeriod>>() : null;
			model.NomKPICategory = (viewKPICategoryUnits.IsSuccess == true) ? viewKPICategoryUnits.Result.To<object, IEnumerable<NomKPICategory>>() : null;
			model.NomFulfillment = (viewFulfillmentCategoryUnits.IsSuccess == true) ? viewFulfillmentCategoryUnits.Result.To<object, IEnumerable<NomFulfillment>>() : null;

			return View(model);
		}

		/// <summary>
		/// Create new page type
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Create(KPI model)
		{
			if (model != null)
			{
				var req = await _service.AddSystem(model);
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
			var model = await _service.GetByIdSystem<KPI, UpdateKPIViewModel>(id);

			var measurementsItemsCategory= await  _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Measurements Items Category");
			var viewMeasurementUnits = (measurementsItemsCategory.IsSuccess == true) ? await _service.Table("NomMeasurement").GetAll<dynamic>(x => x.NomenclatorId == measurementsItemsCategory.Result.FirstOrDefault().Id) : null;


			var calculationPeriodCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Calculation period Category");
			var viewCalculationPeriodUnits = (calculationPeriodCategory.IsSuccess == true) ? await _service.Table("NomPeriod").GetAll<dynamic>(x => x.NomenclatorId == calculationPeriodCategory.Result.FirstOrDefault().Id) : null;

			var kPICategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "KPI Category");
			var viewKPICategoryUnits = (kPICategory.IsSuccess == true) ? await _service.Table("NomKPICategory").GetAll<dynamic>(x => x.NomenclatorId == kPICategory.Result.FirstOrDefault().Id) : null;

			var selectedMeasurementItem = viewMeasurementUnits.Result.Where(x => x.Id == model.Result.MeasurementUnitId).FirstOrDefault();
			var fulfillmentCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Criterion of fulfillment Category");
			var viewFulfillmentCategoryUnits = (selectedMeasurementItem?.Name == "Boolean") ? ((fulfillmentCategory.IsSuccess == true) ? await _service.Table("NomFulfillment").GetAll<dynamic>(x => x.NomenclatorId == fulfillmentCategory.Result.FirstOrDefault().Id  & x.DependencyId == selectedMeasurementItem.Id) : null) : ((fulfillmentCategory.IsSuccess == true) ? await _service.Table("NomFulfillment").GetAll<dynamic>(x => x.NomenclatorId == fulfillmentCategory.Result.FirstOrDefault().Id ) : null);
			

			model.Result.MeasurementUnits = (viewMeasurementUnits.IsSuccess == true) ? viewMeasurementUnits.Result.To<object, IEnumerable<NomMeasurement>>() : null;
			model.Result.NomPeriod = (viewCalculationPeriodUnits.IsSuccess == true) ? viewCalculationPeriodUnits.Result.To<object, IEnumerable<NomPeriod>>() : null;
			model.Result.NomKPICategory = (viewKPICategoryUnits.IsSuccess == true) ? viewKPICategoryUnits.Result.To<object, IEnumerable<NomKPICategory>>() : null;
			model.Result.NomFulfillment = (viewFulfillmentCategoryUnits.IsSuccess == true) ? viewFulfillmentCategoryUnits.Result.To<object, IEnumerable<NomFulfillment>>() : null;

			if (!model.IsSuccess) return NotFound();

			return View(model.Result);
		}

		/// <summary>
		/// Edit page type
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Edit(UpdateKPIViewModel model)
		{
			if (model == null) return NotFound();
			var dataModel = (await _service.GetByIdSystem<KPI, KPI>(model.Id)).Result;

			if (dataModel == null) return NotFound();

			
			dataModel.Name = model.Name;
			dataModel.Description = model.Description;
			dataModel.Author = model.Author;
			dataModel.Changed = DateTime.Now;
			dataModel.FulfillmentCriterionId = model.FulfillmentCriterionId;
			dataModel.MeasurementUnitId = model.MeasurementUnitId;
			dataModel.CategoryId = model.CategoryId;
			dataModel.PeriodId = model.PeriodId;
			dataModel.ProcentGoal = model.ProcentGoal;
			dataModel.BoolGoal = model.BoolGoal;
			dataModel.IntGoal = model.IntGoal;
			dataModel.Status = model.Status;
			var req = await _service.UpdateSystem(dataModel);
			if (req.IsSuccess) return RedirectToAction("Index");
			ModelState.AddModelError(string.Empty, "Fail to save");
			return RedirectToAction(nameof(Index));

		}

		/// <summary>
		/// Get KPI items
		/// </summary>
		/// <param name="KPIId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> GetKPI(Guid KPIId, Guid? parentId = null)
		{
			ViewBag.KPIId = KPIId;
			ViewBag.ParentId = parentId;
			ViewBag.KPI = (await _service.GetByIdSystem<KPI, KPI>(KPIId)).Result;
			
			return View();
		}

		/// <summary>
		/// Create KPI item
		/// </summary>
		/// <param name="KPIId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult CreateItem(Guid KPIId, Guid? parentId = null)
		{
			ViewBag.KPIId = KPIId;
			ViewBag.ParentId = parentId;
			ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
			return View();
		}

		/// <summary>
		/// Create KPI item
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		//[HttpPost]
		//public async Task<IActionResult> CreateItem(KPIItem model)
		//{
		//	ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
		//	if (model != null)
		//	{
		//		//		model.AllowedRoles = "Administrator#";
		//		var req = await _service.AddSystem(model);
		//		if (req.IsSuccess)
		//			return RedirectToAction("GetKPI", new
		//			{
		//				model.KPIId,
		//				ParentId = model.ParentId
		//			});
		//		ModelState.AddModelError(string.Empty, "Fail to save!");
		//	}

		//	return View(model);
		//}

		/// <summary>
		/// Edit item
		/// </summary>
		/// <param name="itemId"></param>
		/// <returns></returns>
		//[HttpGet]
		//public async Task<IActionResult> EditItem(Guid itemId)
		//{
		//	ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
		//	var item = await _service.GetByIdSystem<KPIItem, KPIItem>(itemId);
		//	if (!item.IsSuccess) return NotFound();
		//	return View(item.Result);
		//}

		/// <summary>
		/// Edit item
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		//[HttpPost]
		//public async Task<IActionResult> EditItem(KPIItem model)
		//{
		//	var rq = await _service.UpdateSystem(model);
		//	if (rq.IsSuccess)
		//	{
		//		return RedirectToAction("GetKPI", new
		//		{
		//			model.KPIId,
		//			//		ParentId = model.ParentKPIItemId
		//		});
		//	}

		//	ViewBag.Routes = _context.Pages.Where(x => !x.IsDeleted && !x.IsLayout).Select(x => x.Path);
		//	ModelState.AddModelError(string.Empty, "Fail to save!");
		//	return View(model);
		//}

		/// <summary>
		/// Load KPI items
		/// </summary>
		/// <param name="param"></param>
		/// <param name="KPIId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		//[HttpPost]
		//public async Task<JsonResult> LoadKPIItems(DTParameters param, Guid KPIId, Guid? parentId = null)
		//{
			
		//	var filtered = await _service.Filter<KPIItem>(param.Search.Value, param.SortOrder, param.Start,
		//		param.Length, x => x.KPIId.Equals(KPIId) && x.ParentId.Equals(parentId));

		//	var finalResult = new DTResult<KPIItem>
		//	{
		//		draw = param.Draw,
		//		data = filtered.Item1,
		//		recordsFiltered = filtered.Item2,
		//		recordsTotal = filtered.Item1.Count()
		//	};
		//	return Json(finalResult);
		//}

		/// <summary>
		/// Load page types with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		[AjaxOnly]
		public async Task<JsonResult> LoadPages(DTParameters param)
		{
			var filtered = await _service.Filter<KPI>(param.Search.Value, param.SortOrder, param.Start,
				param.Length);
			var viewModel = filtered.Item1.To<object, List<UpdateKPIViewModel>>();
			foreach (var item in viewModel)
			{
				var muId= await _service.GetByIdSystem<NomMeasurement, NomMeasurement>(item.MeasurementUnitId);
				if (muId.IsSuccess)
				{
					item.SelectedMeasurementUnit = muId.Result.Name;
				}

				var cId = await _service.GetByIdSystem<NomKPICategory, NomKPICategory>(item.CategoryId);
				if (cId.IsSuccess)
				{
					item.SelectedCategory = cId.Result.Name;
				}

				var pId = await _service.GetByIdSystem<NomPeriod, NomPeriod>(item.PeriodId);
				if (pId.IsSuccess)
				{
					item.SelectedPeriod = pId.Result.Name;
				}

				var fId = await _service.GetByIdSystem<NomFulfillment, NomFulfillment>(item.FulfillmentCriterionId);
				if (fId.IsSuccess)
				{
					item.SelectedFulfillmentCriterion = fId.Result.Name;
				}
			}
			var finalResult = new DTResult<UpdateKPIViewModel>
			{
				draw = param.Draw,
				data = viewModel,
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
		public async Task<JsonResult> LoadDependencyNomenclator(DTParameters param,string Id)
		{
			var measurementsItemsCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Measurements Items Category");
			var viewMeasurementUnits = (measurementsItemsCategory.IsSuccess == true) ? await _service.Table("NomenclatorItem").GetAll<dynamic>(x => x.NomenclatorId == measurementsItemsCategory.Result.FirstOrDefault().Id) : null;

			var selectedMeasurementItem = viewMeasurementUnits.Result.Where(x => x.Id == Guid.Parse(Id)).FirstOrDefault();
			var fulfillmentCategory = await _service.Table("Nomenclator").GetAll<dynamic>(x => x.Name == "Criterion of fulfillment Category");
			var viewFulfillmentCategoryUnits = (selectedMeasurementItem?.Name == "Boolean") ? ((fulfillmentCategory.IsSuccess == true) ? await _service.Table("NomenclatorItem").GetAll<dynamic>(x => x.NomenclatorId == fulfillmentCategory.Result.FirstOrDefault().Id & x.DependencyId == selectedMeasurementItem.Id) : null) : ((fulfillmentCategory.IsSuccess == true) ? await _service.Table("NomenclatorItem").GetAll<dynamic>(x => x.NomenclatorId == fulfillmentCategory.Result.FirstOrDefault().Id & x.DependencyId == Guid.Empty) : null);


			//var finalResult = new DTResult<NomenclatorItem>
			//{
			//	draw = param.Draw,
			//	data = viewFulfillmentCategoryUnits.Result.To<object, List<NomenclatorItem>>(),

			//};
			List<NomenclatorItem> finalResult = viewFulfillmentCategoryUnits.Result.To<object, List<NomenclatorItem>>();
			var json = JsonConvert.SerializeObject(finalResult);
			return Json(json);
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
			if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete KPI!", success = false });
			var KPI = await _service.DeletePermanent<KPI>(Guid.Parse(id));
			if (!KPI.IsSuccess) return Json(new { message = "Fail to delete KPI!", success = false });

			return Json(new { message = "KPI was delete with success!", success = true });
		}


		/// <summary>
		/// Delete page type by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		//[Route("api/[controller]/[action]")]
		//[ValidateAntiForgeryToken]
		//[HttpPost, Produces("application/json", Type = typeof(ResultModel))]
		//public async Task<JsonResult> DeleteKPIItem(string id)
		//{
		//	if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete KPI item!", success = false });
		//	var KPI = await _service.DeletePermanent<KPIItem>(Guid.Parse(id));
		//	if (!KPI.IsSuccess) return Json(new { message = "Fail to delete KPI item!", success = false });

		//	return Json(new { message = "Model was delete with success!", success = true });
		//}
	}
}