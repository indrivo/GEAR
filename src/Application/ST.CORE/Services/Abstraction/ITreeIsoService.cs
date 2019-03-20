using System.Collections.Generic;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.CORE.ViewModels.TreeISO;
using ST.Entities.Models.Tables;

namespace ST.CORE.Services.Abstraction
{
	public interface ITreeIsoService
	{
		Task<ResultModel<IEnumerable<TreeStandard>>> LoadTreeStandard(TableModel standardEntity,TableModel categoryEntity, TableModel requirementEntity);
	}
}
