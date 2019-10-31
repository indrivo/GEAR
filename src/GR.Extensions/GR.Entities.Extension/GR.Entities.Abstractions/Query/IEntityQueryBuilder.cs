using System;
using System.Collections.Generic;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;

namespace GR.Entities.Abstractions.Query
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

        /// <summary>
        /// Get pagination result with filters
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        string GetPaginationByFilters(EntityViewModel viewModel, uint page, uint perPage = 10, string queryString = null);

        /// <summary>
        /// Get count by filters
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        string CountByFilters(EntityViewModel viewModel, string queryString = null);
    }
}
