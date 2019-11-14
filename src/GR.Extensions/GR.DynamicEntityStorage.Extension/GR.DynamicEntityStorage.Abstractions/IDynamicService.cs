using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.DynamicEntityStorage.Abstractions.Helpers;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;

namespace GR.DynamicEntityStorage.Abstractions
{
    public interface IDynamicService : IDynamicDataGetService, IDynamicDataCreateService, IDynamicDataUpdateService
    {
        /// <summary>
        /// Register in memory
        /// </summary>
        /// <returns></returns>
        Task RegisterInMemoryDynamicTypesAsync();

        /// <summary>
        /// Check if exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel> Exists<TEntity>(Guid id) where TEntity : BaseModel;

        /// <summary>
        /// Create entity definition
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        Task<TViewModel> CreateEntityDefinition<TEntity, TViewModel>(string tableSchema) where TEntity : BaseModel
            where TViewModel : EntityViewModel;

        /// <summary>
        /// Create entity definition
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        Task<TViewModel> CreateEntityDefinition<TViewModel>(string entityName, string tableSchema) where TViewModel : EntityViewModel;

        /// <summary>
        /// Create entity view  without base model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        Task<TViewModel> CreateWithoutBaseModel<TEntity, TViewModel>() where TEntity : BaseModel where TViewModel : EntityViewModel;
        /// <summary>
        /// Parse from dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        T GetObject<T>(Dictionary<string, object> dict);
        /// <summary>
        /// Parse from dictionary to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionaries"></param>
        /// <returns></returns>

        IEnumerable<T> GetObject<T>(IEnumerable<Dictionary<string, object>> dictionaries);

        /// <summary>
        /// Get Dictionary from object
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        Dictionary<string, object> GetDictionary<TEntity>(TEntity model);

        /// <summary>
        /// Create dynamic table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DynamicObject Table(string tableName);

        /// <summary>
        /// Create dynamic table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        DynamicObject Table<TEntity>() where TEntity : BaseModel;

        /// <summary>
        /// Filter list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<(List<T>, int)> Filter<T>(string search, string sortOrder, int start, int length, Expression<Func<T, bool>> predicate = null) where T : BaseModel;

        /// <summary>
        /// Filter dynamic entity data
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="predicate"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<(List<object>, int)> Filter(string entity, string search, string sortOrder, int start, int length, Expression<Func<object, bool>> predicate = null, IEnumerable<Filter> filters = null);
    }
}
