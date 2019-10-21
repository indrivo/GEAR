using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicDataCreateService
    {
        /// <summary>
        /// Add new value to entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <param name="dbSchema"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> Add<TEntity>(Dictionary<string, object> model, string dbSchema = null) where TEntity : BaseModel;

        /// <summary>
        /// Add new value to entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddWithReflection<TEntity>(TEntity model) where TEntity : BaseModel;

        /// <summary>
        /// Add multiples values to entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Guid>>> AddRange<TEntity>(IEnumerable<Dictionary<string, object>> model) where TEntity : BaseModel;
        /// <summary>
        /// Add Range
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ResultModel<IList<(TEntity, Guid)>>> AddDataRangeWithReflection<TEntity>(IEnumerable<TEntity> data) where TEntity : BaseModel;
    }
}
