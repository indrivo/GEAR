using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.CORE.Services.Abstraction;
using ST.CORE.ViewModels.TreeISOViewModels;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Extensions;
using ST.Entities.Models.Tables;
using ST.Entities.Services.Abstraction;

namespace ST.CORE.Services
{
	public class TreeIsoService : ITreeIsoService
	{
		/// <summary>
		/// Inject Data Service
		/// </summary>
		private readonly IDynamicService _service;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="service"></param>
		public TreeIsoService(IDynamicService service)
		{
			_service = service;
		}

		/// <summary>
		/// Load tree
		/// </summary>
		/// <param name="standardEntity"></param>
		/// <param name="categoryEntity"></param>
		/// <param name="requirementEntity"></param>
		/// <returns></returns>
		public  async Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity, TableModel categoryEntity, TableModel requirementEntity)
		{
			var res = new ResultModel<IEnumerable<TreeStandard>>();
			var standards = await _service.Table(standardEntity.Name).GetAll<dynamic>();
			var tree = new List<TreeStandard>();
			if (!standards.IsSuccess) return res;
			foreach (var standard in standards.Result)
			{
				tree.Add(new TreeStandard
				{
					Name = standard.Name,
					Categories = await LoadCategories(categoryEntity.Name, requirementEntity.Name, standard.Id, Guid.Empty),
					Id = standard.Id
				});
			}

			return res;
		}


		/// <summary>
		/// Load categories
		/// </summary>
		/// <param name="categoryEntity"></param>
		/// <param name="requirementEntity"></param>
		/// <param name="standardId"></param>
		/// <param name="parentCategoryId"></param>
		/// <returns></returns>
		private async Task<IEnumerable<TreeCategory>> LoadCategories(string categoryEntity, string requirementEntity, Guid standardId, Guid parentCategoryId)
		{
			var categories = await _service.Table(categoryEntity).GetAll<dynamic>(x => x.ParentCategoryId == parentCategoryId && x.StandardId == standardId);
			var resCats = new List<TreeCategory>();
			foreach (var category in categories.Result)
			{
				var cat = new TreeCategory
				{
					Name = category.Name,
					Id = category.Id,
					SubCategories = await LoadCategories(categoryEntity, requirementEntity, standardId, category.Id),
					Requirements = await LoadRequirements(requirementEntity, category.Id, Guid.Empty)
				};
				resCats.Add(cat);
			}
			return resCats;
		}


		/// <summary>
		/// Load requirements
		/// </summary>
		/// <param name="requirementEntity"></param>
		/// <param name="categoryId"></param>
		/// <param name="parentRequirementId"></param>
		/// <returns></returns>
		private async Task<IEnumerable<TreeRequirement>> LoadRequirements(string requirementEntity, Guid categoryId, Guid parentRequirementId)
		{
			var res = new List<TreeRequirement>();
			var requirements = await _service.Table(requirementEntity).GetAll<dynamic>(x => x.CategoryId == categoryId && x.ParentRequirementId == parentRequirementId);
			foreach (var req in requirements.Result)
			{
				res.Add(new TreeRequirement
				{
					Name = req.Name,
					Id = req.Id,
					Requirements = await LoadRequirements(requirementEntity, categoryId, req.Id)
				});
			}

			return res;
		}
	}
}
