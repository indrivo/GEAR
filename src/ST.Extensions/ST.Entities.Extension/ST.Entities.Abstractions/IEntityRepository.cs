using System;
using System.Collections.Generic;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.Table;

namespace ST.Entities.Abstractions
{
    public interface IEntityRepository
    {
        ResultModel UpdateTableFieldConfigurations(Guid fieldId, ICollection<FieldConfigViewModel> viewConfigs,
            ICollection<TableFieldConfigValue> dbConfigs);
    }
}
