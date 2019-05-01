using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Enums;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.WebHost.Services.Abstractions;
using ST.WebHost.ViewModels.TreeISOViewModels;

namespace ST.WebHost.Services
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
		public async Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity, TableModel categoryEntity, TableModel requirementEntity)
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
					Categories = await LoadCategories(categoryEntity.Name, requirementEntity.Name, standard.Id, null),
					Id = standard.Id
				});
			}

			res.IsSuccess = true;
			res.Result = tree;
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
		private async Task<IEnumerable<TreeCategory>> LoadCategories(string categoryEntity, string requirementEntity, Guid standardId, Guid? parentCategoryId)
		{
			//x => x.ParentCategoryId == parentCategoryId && x.StandardId == standardId
			var categories = await _service.Table(categoryEntity).GetAll<dynamic>(null, new List<Filter>
			{
				new Filter
				{
					Parameter = "ParentCategoryId",
					Criteria = Criteria.Equals,
					Value = parentCategoryId
				},
				new Filter
				{
					Parameter = "StandardId",
					Criteria = Criteria.Equals,
					Value = standardId
				}
			});
			var resCats = new List<TreeCategory>();
			foreach (var category in categories.Result)
			{
				var cat = new TreeCategory
				{
					Name = category.Name,
					Id = category.Id,
					SubCategories = await LoadCategories(categoryEntity, requirementEntity, standardId, category.Id),
					Requirements = await LoadRequirements(requirementEntity, category.Id, null)
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
		private async Task<IEnumerable<TreeRequirement>> LoadRequirements(string requirementEntity, Guid categoryId, Guid? parentRequirementId)
		{
			var res = new List<TreeRequirement>();
			//x => x.CategoryId == categoryId && x.ParentRequirementId == parentRequirementId
			var requirements = await _service.Table(requirementEntity).GetAll<dynamic>(null,
				new List<Filter>
				{
					new Filter
					{
						Parameter = "CategoryId",
						Criteria = Criteria.Equals,
						Value = categoryId
					},
					new Filter
					{
						Parameter = "ParentRequirementId",
						Criteria = Criteria.Equals,
						Value = parentRequirementId
					}
				});
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
