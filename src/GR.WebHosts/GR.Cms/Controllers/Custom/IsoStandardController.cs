using GR.Cms.Services.Abstractions;
using GR.Core.Helpers;
using GR.Entities.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GR.Cms.Controllers.Custom
{
	[Authorize]
	public class IsoStandardController : Controller
	{
		/// <summary>
		/// DB context
		/// </summary>
		private readonly EntitiesDbContext _context;

		/// <summary>
		/// Inject iso dataService
		/// </summary>
		private readonly ITreeIsoService _isoService;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="isoService"></param>
		/// <param name="context"></param>
		public IsoStandardController(ITreeIsoService isoService, EntitiesDbContext context)
		{
			_isoService = isoService;
			_context = context;
		}

		/// <summary>
		/// Load tree for standards
		/// </summary>
		/// <param name="standardEntityId"></param>
		/// <param name="categoryEntityId"></param>
		/// <param name="requirementEntityId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetTreeData(Guid? standardEntityId, Guid? categoryEntityId,
			Guid? requirementEntityId)
		{
			var result = new ResultModel();
			if (standardEntityId == null || categoryEntityId == null || requirementEntityId == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(nameof(ArgumentNullException), "Tree block configuration is not complete!")
				};
				return Json(result);
			}

			var standardEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == standardEntityId);
			var categoryEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == categoryEntityId);
			var requirementEntity = await _context.Table.FirstOrDefaultAsync(x => x.Id == requirementEntityId);
			if (standardEntity != null && categoryEntity != null && requirementEntity != null)
				return Json(await _isoService.LoadTreeStandard(standardEntity, categoryEntity, requirementEntity));
			result.Errors = new List<IErrorModel>
			{
				new ErrorModel("NotFoundException", "Entities does not exist!")
			};
			return Json(result);
		}

		public async Task<JsonResult> GetControlStructure()
			=> Json(await _isoService.GetControlStructureTree());

		/// <summary>
		/// Add or update requirement fulfillment
		/// </summary>
		/// <param name="requirementId"></param>
		/// <param name="fillRequirementId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> AddOrUpdateStandardRequirementCompleteText([Required] Guid requirementId,
			Guid? fillRequirementId, string value) =>
			Json(await _isoService.AddOrUpdateStandardRequirementCompleteText(requirementId, fillRequirementId, value));

		/// <summary>
		/// Get iso responsibiles from control details
		/// </summary>
		/// <param name="controlDetailsId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetControlResponsibilesAsync([Required] Guid controlDetailsId)
		{
			var data = await _isoService.GetControlResponsibilesAsync(controlDetailsId);
			return Json(new ResultModel
			{
				IsSuccess = true,
				Result = data
			});
		}
	}
}