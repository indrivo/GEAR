using System;
using System.Collections.Generic;
using GR.Entities.Abstractions.Query;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;

namespace GR.Entities.Controls.QueryAbstractions
{
    public abstract class EntityQueryBuilder : IEntityQueryBuilder
    {
        public abstract string DeleteByIdQuery(EntityViewModel viewModel, bool completeDelete = false);
        public abstract string DeleteByIdQuery(EntityViewModel viewModel, Guid parameterId, bool completeDelete = false);
        public abstract string DeleteByParamQuery(string tableName, string parameterName, string parameter,
            bool completeDelete = false);

        public abstract string DeleteByParamQuery(EntityViewModel viewModel, bool completeDelete = false);

        public abstract string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel, string parameterName,
            string parameter, int perPage, int currentPage);

        public abstract string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel, int perPage,
            int currentPage);

        public abstract string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel,
            Dictionary<string, object> parameters, int perPage, int currentPage);

        public abstract string GetByColumnParameterQuery(EntityViewModel viewModel,
            Dictionary<string, object> parameters);

        public abstract string GetByColumnParameterQuery(EntityViewModel viewModel);
        public abstract string GetByIdQuery(EntityViewModel viewModel);
        public abstract string GetCountByParameter(EntityViewModel viewModel, string parameterName, string parameter);
        public abstract string GetCountByParameters(EntityViewModel viewModel);
        public abstract string GetCountByParameter(EntityViewModel viewModel, Dictionary<string, object> parameters);
        public abstract string InsertQuery(EntityViewModel viewModel);
        public abstract string UpdateQuery(EntityViewModel viewModel, Guid parameterId);
        public abstract string UpdateQuery(EntityViewModel viewModel);

        public abstract string GetByIncludeParam(EntityViewModel parentTable, EntityViewModel childTable,
            string fieldName);

        protected abstract string GetWhereString(EntityViewModel viewModel, string nullName = "null",
            string isNullName = "isnull");
    }
}
