using System.Collections.Generic;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.CORE.ViewModels.TreeISOViewModels;
using ST.Entities.Models.Tables;

namespace ST.CORE.Services.Abstractions
{
	public interface ITreeIsoService
	{
		Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity,TableModel categoryEntity, TableModel requirementEntity);
	}
}
