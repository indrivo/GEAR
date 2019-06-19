using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Enums;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Cms.Services.Abstractions;
using ST.Cms.ViewModels.TreeISOViewModels;
using ST.Core;

namespace ST.Cms.Services
{
	public class TreeIsoService : ITreeIsoService
	{
		/// <summary>
		/// Tenant entity name what store tenant standard requirement filfullment method 
		/// </summary>
		private const string ReqFillEntityName = "RequirementFillMethod";

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
			var standards = await _service.Table(standardEntity.Name).GetAllWithInclude<dynamic>(filters: new List<Filter>
			{
				new Filter
				{
					Value = false,
					Criteria = Criteria.Equals,
					Parameter = nameof(BaseModel.IsDeleted)
				}
			});
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
		/// Add or update fullfillment method
		/// </summary>
		/// <param name="requirementId"></param>
		/// <param name="fillRequirementId"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public async Task<ResultModel<Guid>> AddOrUpdateStandardRequirementCompleteText(Guid requirementId, Guid? fillRequirementId, string value)
		{
			var result = new ResultModel<Guid>();
			var ctx = _service.Table(ReqFillEntityName);
			var ctxReq = _service.Table("CategoryRequirements");
			if (requirementId == Guid.Empty)
			{
				result.Errors.Add(new ErrorModel(nameof(ArgumentNullException), "Requirement id not passed"));
				return result;
			}

			if (fillRequirementId == null)
			{
				if (string.IsNullOrEmpty(value))
				{
					result.Errors.Add(new ErrorModel(nameof(ArgumentNullException), "Value must be not null"));
					return result;
				}

				var checkIfExist = await ctx.GetAllWithInclude<dynamic>(filters: new List<Filter>
				{
					new Filter
					{
						Value = requirementId,
						Criteria = Criteria.Equals,
						Parameter = "RequirementId"
					}
				});

				if (checkIfExist.Result?.Any() ?? false)
				{
					result.Errors.Add(new ErrorModel(nameof(NotFoundResult), "Fail to save, try refresh page!"));
					return result;
				}

				var requirement = await ctxReq.GetById<object>(requirementId);
				if (!requirement.IsSuccess || requirement.Result == null)
				{
					result.Errors.Add(new ErrorModel(nameof(NotFoundResult), "Requirement not found by id"));
					return result;
				}

				var o = ctx.GetNewObjectInstance();
				o.ChangePropValue("Value", value);
				o.ChangePropValue("RequirementId", requirementId.ToString());
				var dbResult = await ctx.Add(o);
				return dbResult;
			}

			var data = await ctx.GetAllWithInclude<dynamic>(filters: new List<Filter>
			{
				new Filter
				{
					Value = fillRequirementId,
					Criteria = Criteria.Equals,
					Parameter = nameof(BaseModel.Id)
				}
			});

			if (data.IsSuccess && data.Result.Any())
			{
				if (data.Result.Count() > 2)
				{
					result.Errors.Add(new ErrorModel(nameof(NullReferenceException), "Something went wrong, multiple values detected!"));
					return result;
				}

				var obj = data.Result.FirstOrDefault();
				if (obj == null)
				{
					result.Errors.Add(new ErrorModel(nameof(NullReferenceException), "Something went wrong!"));
					return result;
				}

				if (obj.Value.ToString() == value)
				{
					result.IsSuccess = true;
					result.Result = fillRequirementId.Value;
					return result;
				}
				var toSave = ((object)obj).ParseDynamicObjectByType(ctx.Type);
				toSave.ChangePropValue("Value", value);
				var dbUpdateResult = await ctx.Update(toSave);
				return dbUpdateResult;
			}

			result.Errors = data.Errors;

			return result;
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
				},
				new Filter
				{
					Value = false,
					Criteria = Criteria.Equals,
					Parameter = nameof(BaseModel.IsDeleted)
				}
			});
			var resCats = new List<TreeCategory>();
			foreach (var category in categories.Result.OrderBy(x => x.Name))
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
			var requirements = await _service.Table(requirementEntity).GetAllWithInclude<dynamic>(null,
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
					},
					new Filter
					{
						Value = false,
						Criteria = Criteria.Equals,
						Parameter = nameof(BaseModel.IsDeleted)
					}
				});
			var dueModeCtx = _service.Table(ReqFillEntityName);
			foreach (var req in requirements.Result.OrderBy(x => x.Name))
			{
				var requirement = new TreeRequirement
				{
					Name = req.Name,
					Id = req.Id,
					Hint = req.Hint ?? string.Empty,
					Requirements = await LoadRequirements(requirementEntity, categoryId, req.Id),
					Documents = new List<TreeRequirementDocument>(),

				};

				var rq = await dueModeCtx.GetAll<dynamic>(filters: new List<Filter>
				{
					new Filter
					{
						Parameter = "RequirementId",
						Value = req.Id,
						Criteria = Criteria.Equals
					}
				});
				if (rq.IsSuccess)
				{
					var dueMode = rq.Result?.FirstOrDefault();
					requirement.RequirementDueMode = dueMode == null
						? new RequirementDueMode()
						: new RequirementDueMode
						{
							DueModeId = dueMode.Id,
							DueModeValue = dueMode.Value
						};
				}

				res.Add(requirement);
			}

			return res;
		}
	}
}
