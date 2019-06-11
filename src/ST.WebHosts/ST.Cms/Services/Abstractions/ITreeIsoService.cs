using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Cms.ViewModels.TreeISOViewModels;

namespace ST.Cms.Services.Abstractions
{
	public interface ITreeIsoService
	{
		Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity,TableModel categoryEntity, TableModel requirementEntity);

		Task<ResultModel<Guid>> AddOrUpdateStandardRequirementCompleteText(Guid requirementId, Guid? fillRequirementId,
			string value);
	}
}
