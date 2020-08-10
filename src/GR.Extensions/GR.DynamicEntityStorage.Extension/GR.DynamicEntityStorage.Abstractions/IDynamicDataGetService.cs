using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.Entities.Abstractions.Enums;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicDataGetService
    {
        /// <summary>
        /// Get all with include as dictionary
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAllWithIncludeAsDictionaryAsync(string entity,
            Expression<Func<Dictionary<string, object>, bool>> expression = null, IEnumerable<Filter> filters = null);

        /// <summary>
        /// Get all
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAllAsync(string entity,
            Expression<Func<Dictionary<string, object>, bool>> expression = null, IEnumerable<Filter> filters = null);

        /// <summary>
        /// Get by id with include
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, object>>> GetByIdWithIncludeAsync(string entity, Guid id);

        /// <summary>
        /// Get by id as dictionary
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, object>>> GetByIdAsync(string entity, Guid id);

        /// <summary>
        /// Get paginated result
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="queryString"></param>
        /// <param name="filters"></param>
        /// <param name="orderDirection"></param>
        /// <param name="loadIncludes"></param>
        /// <returns></returns>
        Task<ResultModel<PaginationResponseViewModel>> GetPaginatedResultAsync(string entity, uint page = 1,
            uint perPage = 10, string queryString = null,
            IEnumerable<Filter> filters = null, Dictionary<string, EntityOrderDirection> orderDirection = null, bool loadIncludes = true);

        /// <summary>
        /// Any async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResultModel<bool>> AnyAsync(string entity);

        /// <summary>
        /// Count 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<ResultModel<int>> CountAsync(string entity, Dictionary<string, object> filters = null);
    }
}
