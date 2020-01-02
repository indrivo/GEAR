using GR.Cms.ViewModels.TreeISOViewModels;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GR.Cms.Services.Abstractions
{
	public interface ITreeIsoService
	{
		Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity, TableModel categoryEntity, TableModel requirementEntity);

		Task<ResultModel<Guid>> AddOrUpdateStandardRequirementCompleteText(Guid requirementId, Guid? fillRequirementId,
			string value);

		Task<ResultModel<IEnumerable<ControlRootTree>>> GetControlStructureTree();

		Task<IEnumerable<ControlResponsible>> GetControlResponsibilesAsync(Guid controlDetailId);
	}
}