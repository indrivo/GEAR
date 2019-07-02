using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.Table;

namespace ST.Entities.Abstractions
{
    public interface IEntityRepository
    {
        IQueryable<TableModel> Tables { get; }

        ResultModel UpdateTableFieldConfigurations(Guid fieldId, ICollection<FieldConfigViewModel> viewConfigs,
            ICollection<TableFieldConfigValue> dbConfigs);

        Task<ResultModel<IEnumerable<FieldConfigViewModel>>> RetrieveConfigurationsOnAddNewTableFieldAsyncTask(
            CreateTableFieldViewModel field);

        Task<ResultModel<CreateTableFieldViewModel>> GetAddFieldCreateViewModel(Guid id, string type);
    }
}
