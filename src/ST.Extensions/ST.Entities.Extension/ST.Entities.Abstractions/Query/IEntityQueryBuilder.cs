using System;
using System.Collections.Generic;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;

namespace ST.Entities.Abstractions.Query
{
    public interface IEntityQueryBuilder
    {
        string DeleteByIdQuery(EntityViewModel viewModel, bool completeDelete = false);
        string DeleteByIdQuery(EntityViewModel viewModel, Guid parameterId, bool completeDelete = false);
        string DeleteByParamQuery(string tableName, string parameterName, string parameter,
           bool completeDelete = false);

        string DeleteByParamQuery(EntityViewModel viewModel, bool completeDelete = false);

        string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel, string parameterName,
           string parameter, int perPage, int currentPage);

        string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel, int perPage,
           int currentPage);

        string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel,
           Dictionary<string, object> parameters, int perPage, int currentPage);

        string GetByColumnParameterQuery(EntityViewModel viewModel,
           Dictionary<string, object> parameters);

        string GetByColumnParameterQuery(EntityViewModel viewModel);
        string GetByIdQuery(EntityViewModel viewModel);
        string GetCountByParameter(EntityViewModel viewModel, string parameterName, string parameter);
        string GetCountByParameters(EntityViewModel viewModel);
        string GetCountByParameter(EntityViewModel viewModel, Dictionary<string, object> parameters);
        string InsertQuery(EntityViewModel viewModel);
        string UpdateQuery(EntityViewModel viewModel, Guid parameterId);
        string UpdateQuery(EntityViewModel viewModel);

        string GetByIncludeParam(EntityViewModel parentTable, EntityViewModel childTable,
           string fieldName);
    }
}
