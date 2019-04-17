using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.Shared;

namespace ST.DynamicEntityStorage.Abstractions
{
    public interface IDynamicDataGetService
    {
        Task<ResultModel<IEnumerable<TOutput>>> GetAll<TEntity, TOutput>(Func<TOutput, bool> predicate = null)
            where TEntity : ExtendedModel;

        /// <summary>
        /// Get all from entity
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>(Expression<Func<TEntity, bool>> predicate = null, IEnumerable<Filter> filters = null) where TEntity : ExtendedModel;

        /// <summary>
        /// Get all from entity
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TOutput>>> GetAllWithInclude<TEntity, TOutput>(Func<TEntity, bool> predicate = null, IEnumerable<Filter> filters = null) where TEntity : ExtendedModel;

        /// <summary>
        /// Get all with predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>

        Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>(Func<Dictionary<string, object>, bool> func) where TEntity : ExtendedModel;
        /// <summary>
        /// Get paginated result
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetPaginated<TEntity>(ulong page = 1, ulong perPage = 10) where TEntity : ExtendedModel;

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Dictionary<string, object>>> GetById<TEntity>(Guid id) where TEntity : ExtendedModel;

        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<TOutput>> GetByIdSystem<TEntity, TOutput>(Guid id) where TEntity : ExtendedModel;

        /// <summary>
        /// Get table configurations
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TTable"></typeparam>
        /// <returns></returns>
        Task<ResultModel<TTable>> GetTableConfigurations<TEntity, TTable>()
            where TEntity : ExtendedModel where TTable : class;

        /// <summary>
        /// Check any data in table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<ResultModel<bool>> Any<TEntity>() where TEntity : ExtendedModel;

        /// <summary>
        /// Count data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Task<ResultModel<int>> Count<TEntity>() where TEntity : ExtendedModel;

        /// <summary>
        /// Get first or default
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<ResultModel<TEntity>> FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ExtendedModel;

        /// <summary>
        /// Get last or default
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<ResultModel<TEntity>> LastOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ExtendedModel;
    }
}
