using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicDataUpdateService
    {
        /// <summary>
        /// Update record
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> UpdateAsync(string entity, Dictionary<string, object> model);

        /// <summary>
        /// Delete row permanent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> DeletePermanentAsync(string entity, Guid id);

        /// <summary>
        /// Change status to deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> DeleteAsync(string entity, Guid id);

        /// <summary>
        /// Restore item
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> RestoreAsync(string entity, Guid id);
    }
}