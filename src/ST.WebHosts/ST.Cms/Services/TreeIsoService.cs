using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Cms.Services.Abstractions;
using ST.Cms.ViewModels.TreeISOViewModels;
using ST.Core;
using ST.Core.Helpers.Comparers;
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
#pragma warning disable 1998

namespace ST.Cms.Services
{
	public sealed class TreeIsoService : ITreeIsoService
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

		#region  Standart Structure
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
			var standards = await _service.Table(standardEntity.Name).GetAllWithInclude<dynamic>(filters: new Collection<Filter>
			{
				new Filter(nameof(BaseModel.IsDeleted), false)
			});

			var tree = new Collection<TreeStandard>();
			if (!standards.IsSuccess) return res;
			foreach (var standard in standards.Result.ToList())
			{
				Guid standardId = standard.Id;
				var categories = await LoadCategories(categoryEntity.Name, requirementEntity.Name, standardId, null);
				tree.Add(new TreeStandard
				{
					Name = standard.Name,
					Categories = categories?.ToList(),
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
			var categories = await _service.Table(categoryEntity).GetAll<dynamic>(null, new List<Filter>
			{
				new Filter("ParentCategoryId", parentCategoryId),
				new Filter("StandardId", standardId),
				new Filter(nameof(BaseModel.IsDeleted), false)
			});

			var result = categories.Result?.OrderBy(x => (string)x.Number, new StringNumberComparer())
				.Select(async category => new TreeCategory
				{
					Number = category.Number,
					Name = category.Name,
					Id = category.Id,
					SubCategories = await LoadCategories(categoryEntity, requirementEntity, standardId, category.Id),
					Requirements = await LoadRequirements(requirementEntity, category.Id, null),
					CategoryActions = await GetCategoryActions(category.Id)
				}).Select(task => task.Result).ToList();

			return result;
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
			var requirements = await _service.Table(requirementEntity).GetAllWithInclude<dynamic>(null,
				new List<Filter>
				{
					new Filter("CategoryId", categoryId),
					new Filter("ParentRequirementId", parentRequirementId),
					new Filter(nameof(BaseModel.IsDeleted), false)
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
					new Filter("RequirementId", req.Id)
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

		private async Task<TreeCategoryAction> GetCategoryActions(Guid categoryId)
		{
			var result = new TreeCategoryAction();
			//TODO: Attend response from business analyst
			//var filters = new List<Filter>
			//{
			//	new Filter("ControlRecordId", categoryId)
			//};

			//try
			//{
			//	var dbResult = await _service.Table("ActionPlan").GetAllWithInclude<dynamic>(filters: filters);
			//	if (!dbResult.IsSuccess) return result;
			//	foreach (var item in dbResult.Result)
			//	{
			//		if (item.ActionStateReference.Code == (int)ActionState.Closed)
			//		{
			//			++result.ClosedActions;
			//		}
			//		else
			//		{
			//			++result.OpenActions;
			//		}
			//	}
			//}
			//catch (Exception e)
			//{
			//	Debug.WriteLine(e);
			//}
			return result;
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
					new Filter("RequirementId", requirementId)
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
				new Filter( nameof(BaseModel.Id), fillRequirementId)
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
		#endregion


		#region Control Structure

		public async Task<ResultModel<IEnumerable<ControlRootTree>>> GetControlStructureTree()
		{
			const string controlStructureEntity = "ControlStructure";
			var result = new ResultModel<IEnumerable<ControlRootTree>>();
			var controlStructures = await _service.Table(controlStructureEntity).GetAllWithInclude<dynamic>(filters: new List<Filter>
			{
				new Filter( nameof(BaseModel.IsDeleted), false)
			});

			if (!controlStructures.IsSuccess)
			{
				result.Errors = controlStructures.Errors;
			}

			var data = controlStructures.Result?.ToList();
			if (data == null)
			{
				result.Errors.Add(
					new ErrorModel(nameof(ArgumentNullException), "Data is not completed by system administration, try contact your administrator!"));
				return result;
			}

			var root = data.Where(x => x.ParentId == null || x.ParentId == Guid.Empty).ToList();
			var rootResult = root.Select(x =>
			{
				var secondLevels = RetrieveSecondControlLevel(data, (Guid)x.Id).ToList();
				var collapseChilds = string.Join(" ", secondLevels.Select(y => $"_second-level_{y.Id.ToString()}"));
				return new ControlRootTree
				{
					Id = x.Id,
					Name = x.Name,
					Number = x.Nr,
					SecondLevels = secondLevels,
					CollapseSelectors = collapseChilds
				};
			}).OrderBy(x => x.Number, new StringNumberComparer()).ToList();

			result.IsSuccess = true;
			result.Result = rootResult;
			return result;
		}

		/// <summary>
		/// Get control details
		/// </summary>
		/// <param name="controlId"></param>
		/// <returns></returns>
		private async Task<ControlDetails> GetControlDetailsAsync(Guid controlId)
		{
			var def = new ControlDetails
			{
				Responsibles = new List<ControlResponsible>()
			};
			const string controlEntity = "ControlDetails";
			var controlDetails = await _service.Table(controlEntity).GetAll<dynamic>(filters: new List<Filter>
			{
				new Filter(nameof(BaseModel.IsDeleted), false),
				new Filter("ControlStructureId", controlId)
			});
			if (!controlDetails.IsSuccess || !controlDetails.Result.Any()) return def;
			var item = controlDetails.Result.FirstOrDefault();
			if (item != null)
			{
				return new ControlDetails
				{
					ControlDetailId = item.Id,
					Applicability = item.Application,
					Implemented = item.Applied,
					Comments = item.Comments,
					Details = item.Details,
					Motivation = item.Motivation,
					Responsibles = await GetControlResponsibilesAsync(item.Id)
				};
			}
			return def;
		}

		/// <summary>
		/// Get control responsibilities
		/// </summary>
		/// <param name="controlDetailId"></param>
		/// <returns></returns>
		public async Task<IEnumerable<ControlResponsible>> GetControlResponsibilesAsync(Guid controlDetailId)
		{
			var rs = new List<ControlResponsible>();
			const string controlEntity = "ControlDetailsReponsible";
			var controlResponsibiles = await _service.Table(controlEntity).GetAllWithInclude<dynamic>(filters: new List<Filter>
			{
				new Filter(nameof(BaseModel.IsDeleted), false),
				new Filter("ControlDetailId", controlDetailId)
			});
			if (!controlResponsibiles.IsSuccess) return rs;
			if (!controlResponsibiles.Result.Any()) return rs;
			foreach (var item in controlResponsibiles.Result)
			{
				string[] info = GetPersonInfo(item.PersonIdReference);
				rs.Add(new ControlResponsible
				{
					Id = item.Id,
					ControlDetailId = controlDetailId,
					NomPersonId = item.PersonIdReference?.Id,
					FullName = info[1],
					Initials = info[0],
					BackgroundColor = info[2]
				});
			}
			return rs;
		}

		/// <summary>
		/// Get person info
		/// </summary>
		/// <param name="person"></param>
		/// <returns></returns>
		private static string[] GetPersonInfo(dynamic person)
		{
			try
			{
				var fullName = $"{person?.Name} {person?.LastName}";
				var firstNameInitial = ((string)person?.Name.ToString()?[0].ToString())?.ToUpperInvariant();
				var lastNameInitial = ((string)person?.LastName.ToString()?[0].ToString())?.ToUpperInvariant();
				var initials = $"{firstNameInitial}{lastNameInitial}";
				var color = GetRandomColor();
				return new[] { initials, fullName, color };
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
			return new[] { "", "", "" };
		}

		/// <summary>
		/// Get random color
		/// </summary>
		/// <returns></returns>
		private static string GetRandomColor()
		{
			var random = new Random();
			var color = $"#{random.Next(0x1000000):X6}";
			return color;
		}


		/// <summary>
		/// Get second level for controls
		/// </summary>
		/// <param name="source"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		private IEnumerable<ControlSecondLevel> RetrieveSecondControlLevel(IReadOnlyCollection<dynamic> source, Guid parentId)
		{
			var data = source.Where(x => x.ParentId == parentId).Select(async x =>
			{
				var (goal, _) = await GetControlContentAndGoalAsync((Guid)x.Id);
				var thirdLevels = RetrieveThirdLevel(source, (Guid)x.Id).ToList();
				var collapseChilds = string.Join(" ", thirdLevels.Select(y => $"_third-level_{y.Id.ToString()}"));
				return new ControlSecondLevel
				{
					ParentId = parentId,
					Number = x.Nr,
					Name = x.Name,
					Id = x.Id,
					Goal = goal,
					ThirdLevels = thirdLevels,
					CollapseSelectors = collapseChilds
				};
			}).Select(x => x.Result).OrderBy(x => x.Number, new StringNumberComparer()).ToList();
			return data;
		}

		/// <summary>
		/// Get third level
		/// </summary>
		/// <param name="source"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		private IEnumerable<ControlThirdLevel> RetrieveThirdLevel(IReadOnlyCollection<dynamic> source, Guid parentId)
		{
			var data = source.Where(x => x.ParentId == parentId).Select(async x =>
			{
				var (_, content) = await GetControlContentAndGoalAsync((Guid)x.Id);
				return new ControlThirdLevel
				{
					ParentId = parentId,
					Number = x.Nr,
					Name = x.Name,
					Id = x.Id,
					Content = content,
					ControlActivities = await GetControlActivitiesAsync(x.Id),
					ControlDocuments = new ControlDocuments(),
					ControlRisks = new ControlRisks
					{
						TotalRisks = await GetControlRisksCount(x.Id)
					},
					ControlDetails = await GetControlDetailsAsync(x.Id)
				};
			}).Select(x => x.Result).OrderBy(x => x.Number, new StringNumberComparer()).ToList();
			return data;
		}

		/// <summary>
		/// Get control activities counts
		/// </summary>
		/// <param name="controlId"></param>
		/// <returns></returns>
		private async Task<ControlActivities> GetControlActivitiesAsync(Guid controlId)
		{
			var result = new ControlActivities();
			var filters = new List<Filter>
			{
				new Filter("ControlRecordId", controlId),
				new Filter("Source", Guid.Parse("ae735839-020f-4b66-9128-80cd486ec719"))
			};

			try
			{
				var dbResult = await _service.Table("ActionPlan").GetAllWithInclude<dynamic>(filters: filters);
				if (!dbResult.IsSuccess) return result;
				foreach (var item in dbResult.Result)
				{
					if (item.ActionStateReference.Code == (int)ActionState.Closed)
					{
						++result.ClosedActivities;
					}
					else
					{
						++result.OpenActivities;
					}
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
			return result;
		}

		/// <summary>
		/// Get total risks assigned to control
		/// </summary>
		/// <param name="controlId"></param>
		/// <returns></returns>
		private async Task<int> GetControlRisksCount(Guid controlId)
		{
			var result = 0;
			var filters = new Dictionary<string, object>
			{
				{ "ControlId", controlId }
			};
			var dbResult = await _service.Table("RiskControls").Count(filters);
			if (dbResult.IsSuccess)
			{
				result = dbResult.Result;
			}
			return result;
		}

		/// <summary>
		/// Get content and goal parameters for second and third level of control
		/// </summary>
		/// <param name="controlId"></param>
		/// <returns></returns>
		private async Task<(string, string)> GetControlContentAndGoalAsync(Guid controlId)
		{
			const string controlEntity = "Control";
			var controlSecondLevel = await _service.Table(controlEntity).GetAll<dynamic>(filters: new List<Filter>
			{
				new Filter(nameof(BaseModel.IsDeleted), false),
				new Filter("ControlStructureId", controlId)
			});
			if (!controlSecondLevel.IsSuccess) return (string.Empty, string.Empty);
			var item = controlSecondLevel.Result?.FirstOrDefault();
			return item != null ? ((string, string))(item.Goal, item.Content) : (string.Empty, string.Empty);
		}

		#endregion

		#region Helpers
		private enum ActionState
		{
			Closed, InProgress, Open
		}
		#endregion
	}
}
