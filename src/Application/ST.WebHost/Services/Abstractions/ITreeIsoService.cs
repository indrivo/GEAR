using System.Collections.Generic;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.Entities.Abstractions.Models.Tables;
using ST.WebHost.ViewModels.TreeISOViewModels;

namespace ST.WebHost.Services.Abstractions
{
	public interface ITreeIsoService
	{
		Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity,TableModel categoryEntity, TableModel requirementEntity);
	}
}
