using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;
using GR.Identity.Abstractions.Models.MultiTenants;

namespace GR.Entities.Abstractions
{
    public interface IEntityRepository
    {
        IQueryable<TableModel> Tables { get; }

        ResultModel UpdateTableFieldConfigurations(Guid fieldId, ICollection<FieldConfigViewModel> viewConfigs,
            ICollection<TableFieldConfigValue> dbConfigs);

        Task<ResultModel<IEnumerable<FieldConfigViewModel>>> RetrieveConfigurationsOnAddNewTableFieldAsyncTask(
            CreateTableFieldViewModel field);

        Task<ResultModel<CreateTableFieldViewModel>> GetAddFieldCreateViewModel(Guid id, string type);
        /// <summary>
        /// Create dynamic tables
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="schemaName"></param>
        Task CreateDynamicTablesFromInitialConfigurationsFile(Guid tenantId, string schemaName = null);

        /// <summary>
        /// Table field configurations
        /// </summary>
        /// <param name="field"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        Task<IEnumerable<FieldConfigViewModel>> GetTableFieldConfigurations(TableModelField field, string schema);

        /// <summary>
        /// Create tables by replicate system schema
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        Task CreateDynamicTablesByReplicateSchema(Guid tenantId, string schemaName = null);

        /// <summary>
        /// Duplicate tables per schema
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        Task DuplicateEntitiesForSchema(string schema);

        /// <summary>
        /// Get table fields for builder mode
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        Task<IEnumerable<CreateTableFieldViewModel>> GetTableFieldsForBuildMode(TableModel table);

        /// <summary>
        /// Get table configuration
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        Task<ResultModel<SynchronizeTableViewModel>> GetTableConfiguration(Guid tableId, TableModel tableModel = null);

        /// <summary>
        /// Generate tables for tenant
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> GenerateTablesForTenantAsync(Tenant model);

        /// <summary>
        /// Find table by id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<ResultModel<TableModel>> FindTableByNameAsync(string name);
    }
}
