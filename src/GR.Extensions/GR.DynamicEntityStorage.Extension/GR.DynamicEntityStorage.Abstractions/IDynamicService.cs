using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicService : IDynamicDataGetService, IDynamicDataCreateService, IDynamicDataUpdateService
    {
        /// <summary>
        /// Check if exists
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> ExistsAsync(string entity, Guid id);

        /// <summary>
        /// Create entity definition
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        Task<TViewModel> CreateEntityDefinitionAsync<TViewModel>(string entityName, string tableSchema) where TViewModel : EntityViewModel;
    }
}
