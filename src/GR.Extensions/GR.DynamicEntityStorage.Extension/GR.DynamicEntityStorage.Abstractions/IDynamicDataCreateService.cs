using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicDataCreateService
    {
        /// <summary>
        /// Add new value to entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <param name="dbSchema"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddAsync(string entity, Dictionary<string, object> model, string dbSchema = null);

        /// <summary>
        /// Add multiples values to entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Guid>>> AddRangeAsync(string entity, IEnumerable<Dictionary<string, object>> model);
    }
}
