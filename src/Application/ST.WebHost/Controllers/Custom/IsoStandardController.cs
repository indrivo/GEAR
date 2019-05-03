using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Core.Helpers;
using ST.Entities.Data;
using ST.WebHost.Services.Abstractions;

namespace ST.WebHost.Controllers.Custom
{
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
		public async Task<JsonResult> GetTreeData(Guid? standardEntityId, Guid? categoryEntityId, Guid? requirementEntityId)
		{
			var result = new ResultModel();
			if (standardEntityId == null || categoryEntityId == null || requirementEntityId == null)
			{
				result.Errors = new List<IErrorModel>
				{
					new ErrorModel(Guid.NewGuid().ToString(), "Tree block configuration is not complete!")
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
				new ErrorModel(Guid.NewGuid().ToString(), "Entities does not exist!")
			};
			return Json(result);

		}
	}
}
