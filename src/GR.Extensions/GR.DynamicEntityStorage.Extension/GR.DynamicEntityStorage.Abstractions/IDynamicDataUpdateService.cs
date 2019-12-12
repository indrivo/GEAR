using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicDataUpdateService
    {
        Task<ResultModel<Guid>> Update(string entity, Dictionary<string, object> model);
        /// <summary>
        /// Update model in table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> Update<TEntity>(Dictionary<string, object> model) where TEntity : BaseModel;
        /// <summary>
        /// Update model in table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> UpdateWithReflection<TEntity>(TEntity model) where TEntity : BaseModel;

        /// <summary>
        /// Delete row permanent
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> DeletePermanent<TEntity>(Guid id) where TEntity : BaseModel;

        /// <summary>
        /// Change status to deleted
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> Delete<TEntity>(Guid id) where TEntity : BaseModel;
        /// <summary>
        /// Restore item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> Restore<TEntity>(Guid id) where TEntity : BaseModel;
    }
}
