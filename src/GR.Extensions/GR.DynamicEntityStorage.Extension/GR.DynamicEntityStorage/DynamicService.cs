using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GR.Audit.Abstractions.Enums;
using GR.Audit.Abstractions.Extensions;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Extensions;
using GR.Entities.Controls.Builders;
using GR.Entities.Data;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Enums;
using GR.Entities.Abstractions.Extensions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Entities.Security.Abstractions;
using GR.Entities.Security.Abstractions.Enums;
using GR.Entities.Security.Abstractions.Helpers;
using GR.Identity.Abstractions;

namespace GR.DynamicEntityStorage
{
    /// <inheritdoc />
    public class DynamicService<TContext> : IDynamicService where TContext : EntitiesDbContext, IEntityContext
    {
        #region Injectable
        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly TContext _context;

        /// <summary>
        /// Inject accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject entity access manager
        /// </summary>
        private readonly IEntityRoleAccessService _entityRoleAccessService;

        /// <summary>
        /// Inject entity repository
        /// </summary>
        private readonly IEntityService _entityService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityRoleAccessService"></param>
        /// <param name="userManager"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="entityService"></param>
        public DynamicService(TContext context, IEntityRoleAccessService entityRoleAccessService, IUserManager<GearUser> userManager, IHttpContextAccessor httpContextAccessor, IEntityService entityService)
        {
            _context = context;
            _entityRoleAccessService = entityRoleAccessService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _entityService = entityService;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all  as dictionary collection
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAllAsync(string entity, Expression<Func<Dictionary<string, object>, bool>> expression = null, IEnumerable<Filter> filters = null)
        {
            var result = new ResultModel<IEnumerable<Dictionary<string, object>>>();

            if (string.IsNullOrEmpty(entity)) return result;
            var (schema, state, errorModel, table) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveReadAccessAsync(table.Id)) return new AccessDeniedResult<IEnumerable<Dictionary<string, object>>>();
            result.IsSuccess = true;
            var model = await CreateEntityDefinitionAsync<EntityViewModel>(entity, schema);
            var enumeratedFilters = filters?.ToList() ?? new List<Filter>();
            model.Values = GetFilters(enumeratedFilters);
            model.Filters = enumeratedFilters.ToList();
            var data = _context.ListEntitiesByParams(model);
            if (expression != null) data.Result.Values = data.Result.Values.Where(expression.Compile()).ToList();
            result.Result = data.Result.Values;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all with include as dictionary collection
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAllWithIncludeAsDictionaryAsync(string entity, Expression<Func<Dictionary<string, object>, bool>> expression = null, IEnumerable<Filter> filters = null)
        {
            var listWithoutInclude = await GetAllAsync(entity, expression, filters);
            if (!listWithoutInclude.IsSuccess) return listWithoutInclude;
            var tableRequest = await _entityService.FindTableByNameAsync(entity);
            if (!tableRequest.IsSuccess) return listWithoutInclude;
            var table = tableRequest.Result;
            var fieldReferences = table.TableFields
                .Where(x => x.TableFieldConfigValues.Any(y => y.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable)).ToList();
            if (!fieldReferences.Any()) return listWithoutInclude;
            foreach (var item in listWithoutInclude.Result.ToList())
            {
                var includes = await IncludeSingleForDictionaryObjectAsync(item, fieldReferences);
                if (includes.Any())
                {
                    item.AddRange(includes);
                }
            }

            return listWithoutInclude;
        }

        /// <summary>
        /// Include for single
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldReferences"></param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> IncludeSingleForDictionaryObjectAsync(IReadOnlyDictionary<string, object> item,
            IEnumerable<TableModelField> fieldReferences)
        {
            var additionalProps = new Dictionary<string, object>();
            if (item == null) return additionalProps;
            foreach (var reference in fieldReferences)
            {
                var tableName = reference.TableFieldConfigValues
                    .FirstOrDefault(x => x.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable)?.Value;
                if (!item.ContainsKey(reference.Name)) continue;
                var refId = item[reference.Name];
                if (refId == null)
                {
                    additionalProps.Add($"{reference.Name}Reference", null);
                    continue;
                }
                Guid.TryParse(refId.ToString(), out var parsedId);
                if (parsedId == Guid.Empty)
                {
                    additionalProps.Add($"{reference.Name}Reference", null);
                    continue;
                }
                var obj = await GetByIdAsync(tableName, parsedId);
                additionalProps.Add($"{reference.Name}Reference", obj.Result);
            }

            return additionalProps;
        }

        /// <summary>
        /// Get filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        private static List<Dictionary<string, object>> GetFilters(IEnumerable<Filter> filters)
        {
            if (filters == null) return new List<Dictionary<string, object>>();
            var result = new List<Dictionary<string, object>>();
            var dict = filters.ToDictionary(filter => filter.Parameter, filter => filter.Value);
            result.Add(dict);
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, object>>> GetByIdAsync(string entity, Guid id)
        {
            var result = new ResultModel<Dictionary<string, object>>();
            if (string.IsNullOrEmpty(entity)) return result;
            var (schema, state, errorModel, table) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveReadAccessAsync(table.Id)) return new AccessDeniedResult<Dictionary<string, object>>();
            var model = await CreateEntityDefinitionAsync<EntityViewModel>(entity, schema);
            model.Values = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>{ { nameof(BaseModel.Id), id } }
            };
            var data = _context.ListEntitiesByParams(model);
            result.Result = data.Result.Values.FirstOrDefault();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get by id with include
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, object>>> GetByIdWithIncludeAsync(string entity, Guid id)
        {
            var obj = await GetByIdAsync(entity, id);
            if (!obj.IsSuccess) return obj;
            var tableRequest = await _entityService.FindTableByNameAsync(entity);
            if (!tableRequest.IsSuccess) return obj;
            var table = tableRequest.Result;
            var fieldReferences = table.TableFields
                .Where(x => x.TableFieldConfigValues.Any(y => y.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable)).ToList();
            if (!fieldReferences.Any()) return obj;
            var includes = await IncludeSingleForDictionaryObjectAsync(obj.Result, fieldReferences);
            if (includes.Any())
            {
                obj.Result.AddRange(includes);
            }

            return obj;
        }

        /// <inheritdoc />
        /// <summary>
        /// Implement Any method
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<bool>> AnyAsync(string entity)
        {
            var result = new ResultModel<bool>();
            if (string.IsNullOrEmpty(entity)) return result;
            var count = await CountAsync(entity);
            if (!count.IsSuccess) return result;
            result.Result = count.Result > 0;
            result.IsSuccess = true;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Count all data
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<int>> CountAsync(string entity, Dictionary<string, object> filters = null)
        {
            var result = new ResultModel<int>()
            {
                Result = 0
            };
            if (string.IsNullOrEmpty(entity)) return result;
            var (schema, state, errorModel, table) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }

            if (!await _entityRoleAccessService.HaveReadAccessAsync(table.Id)) return new AccessDeniedResult<int>();
            var model = await CreateEntityDefinitionAsync<EntityViewModel>(entity, schema);
            var count = _context.GetCount(model, filters);
            if (!count.IsSuccess) return result;
            result.IsSuccess = count.IsSuccess;
            result.Result = count.Result;
            return result;
        }

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
        public virtual async Task<ResultModel<PaginationResponseViewModel>> GetPaginatedResultAsync(string entity, uint page = 1, uint perPage = 10, string queryString = null,
            IEnumerable<Filter> filters = null, Dictionary<string, EntityOrderDirection> orderDirection = null, bool loadIncludes = true)
        {
            var response = new ResultModel<PaginationResponseViewModel>();
            if (entity.IsNullOrEmpty()) return response;
            var (schema, state, errorModel, table) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                response.Errors.Add(errorModel);
                return response;
            }

            if (!await _entityRoleAccessService.HaveReadAccessAsync(table.Id))
                return new AccessDeniedResult<PaginationResponseViewModel>();

            var model = await CreateEntityDefinitionAsync<EntityViewModel>(entity, schema);
            if (filters != null) model.Filters = filters.ToList();
            if (orderDirection != null) model.OrderByColumns = orderDirection;
            var dbRequest = _context.GetPaginationResult(model, page, perPage, queryString);
            if (!loadIncludes) return dbRequest;
            var referenceFields = table.TableFields?.Where(x => x.DataType.Equals(TableFieldDataType.Guid)).ToList();
            var responseData = dbRequest.Result.ViewModel.Values;
            if (referenceFields != null)
            {
                foreach (var field in referenceFields)
                {
                    var fieldConfigurations = await _entityService.GetTableFieldConfigurations(field, schema);
                    var tableName = fieldConfigurations?.FirstOrDefault(x => x.ConfigCode.Equals(TableFieldConfigCode.Reference.ForeingTable));
                    if (tableName == null) continue;
                    foreach (var entry in responseData)
                    {
                        var identifier = entry[field.Name]?.ToString().ToGuid();
                        var obj = identifier != null ? await GetByIdAsync(tableName.Value, identifier.Value) : null;
                        entry.Add($"{field.Name}Reference", obj?.Result);
                    }
                }
            }
            dbRequest.Result.ViewModel.Values = responseData;

            return dbRequest;
        }

        /// <inheritdoc />
        /// <summary>
        /// Implement add new item
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <param name="dbSchema"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddAsync(string entity, Dictionary<string, object> model, string dbSchema = null)
        {
            var result = new ResultModel<Guid>();
            if (string.IsNullOrEmpty(entity)) return result;
            var (schema, state, errorModel, dto) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveAccessAsync(dto.Id, EntityAccessType.Write)) return new AccessDeniedResult<Guid>();
            var table = await CreateEntityDefinitionAsync<EntityViewModel>(entity, dbSchema ?? schema);
            var author = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            //Set default values
            model.SetDefaultValues(table);
            model["Changed"] = DateTime.Now;
            model["Created"] = DateTime.Now;
            model["Author"] = author;
            model["ModifiedBy"] = author;

            var audit = model.GetTrackAuditFromDictionary(typeof(EntitiesDbContext).FullName, _userManager.CurrentUserTenantId, entity, TrackEventType.Added);
            if (model.ContainsKey("Version"))
            {
                model["Version"] = audit.Version;
            }
            table.Values = new List<Dictionary<string, object>> { model };
            result = _context.AddEntry(table);
            if (!result.IsSuccess) return result;
            if (audit == null) return result;
            audit.RecordId = result.Result;
            audit.Store(_context);
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Implement add range
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Guid>>> AddRangeAsync(string entity, IEnumerable<Dictionary<string, object>> items)
        {
            var result = new ResultModel<IEnumerable<Guid>>();
            if (string.IsNullOrEmpty(entity)) return result;
            var local = new List<Guid>();
            foreach (var item in items)
            {
                var action = await AddAsync(entity, item);
                local.Add(action.IsSuccess ? action.Result : Guid.Empty);
            }

            result.Result = local;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> UpdateAsync(string entity, Dictionary<string, object> model)
        {
            var result = new ResultModel<Guid>();
            var (schema, state, errorModel, dto) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveAccessAsync(dto.Id, EntityAccessType.Update)) return new AccessDeniedResult<Guid>();
            var table = await CreateEntityDefinitionAsync<EntityViewModel>(entity, schema);
            model["Changed"] = DateTime.Now;
            model["ModifiedBy"] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            try
            {
                model["Version"] = Convert.ToInt32(model["Version"]) + 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            table.Values = new List<Dictionary<string, object>> { model };
            var req = _context.UpdateEntry(table);
            if (!req.IsSuccess || !req.Result) return result;
            result.IsSuccess = true;
            result.Result = Guid.Parse(model["Id"].ToString());
            var audit = model.GetTrackAuditFromDictionary(typeof(EntitiesDbContext).FullName, _userManager.CurrentUserTenantId,
                null, TrackEventType.Updated);

            audit.RecordId = result.Result;
            audit.Store(_context);
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete permanent
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> DeletePermanentAsync(string entity, Guid id)
        {
            var result = new ResultModel<Guid>();
            var (schema, state, errorModel, dto) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveAccessAsync(dto.Id, EntityAccessType.DeletePermanent)) return new AccessDeniedResult<Guid>();
            var table = await CreateEntityDefinitionAsync<EntityViewModel>(entity, schema);
            table.Values = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> {{nameof(BaseModel.Id), id}}
            };
            var res = _context.DeleteById(table, true);
            if (!res.Result)
            {
                result.Errors = res.Errors;
                return result;
            }
            result.IsSuccess = true;
            result.Result = id;
            return result;
        }


        /// <inheritdoc />
        /// <summary>
        /// Change status to is deleted
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> DeleteAsync(string entity, Guid id)
        {
            var result = new ResultModel<Guid>();
            var item = await GetByIdAsync(entity, id);
            if (!item.IsSuccess) return result;
            var model = item.Result;
            model[nameof(BaseModel.IsDeleted)] = true;
            model[nameof(BaseModel.Changed)] = DateTime.Now;
            model[nameof(BaseModel.ModifiedBy)] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            result = await UpdateAsync(entity, model);
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Restore item
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> RestoreAsync(string entity, Guid id)
        {
            var result = new ResultModel<Guid>();
            var item = await GetByIdAsync(entity, id);
            if (!item.IsSuccess) return result;
            var model = item.Result;
            model[nameof(BaseModel.IsDeleted)] = false;
            model[nameof(BaseModel.Changed)] = DateTime.Now;
            model[nameof(BaseModel.ModifiedBy)] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            result = await UpdateAsync(entity, model);
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if exists
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ExistsAsync(string entity, Guid id)
        {
            var result = new ResultModel();
            var item = await GetByIdAsync(entity, id);
            result.IsSuccess = item.IsSuccess;
            return result;
        }

        /// <summary>
        /// Create entity definition
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="entityName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public virtual async Task<TViewModel> CreateEntityDefinitionAsync<TViewModel>(string entityName, string tableSchema) where TViewModel : EntityViewModel
        {
            var model = new EntityViewModel
            {
                TableName = entityName,
                TableSchema = tableSchema,
                Fields = new List<EntityFieldsViewModel>()
            };

            model = await ViewModelBuilderFactory.ResolveAsync(_context, model);
            return (TViewModel)model;
        }

        #region Helpers

        /// <summary>
        /// Get entity info 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        // ReSharper disable once UnusedTupleComponentInReturnValue
        protected virtual async Task<(string, bool, ErrorModel, TableModel)> GetEntityInfoSchemaAsync(string entity)
        {
            var tableRequest = await _entityService.FindTableByNameAsync(entity);
            if (!tableRequest.IsSuccess)
                return (null, false, new ErrorModel("entity_not_found", "Entity not found!"), null);

            return (tableRequest.Result.EntityType, true, null, tableRequest.Result);
        }

        #endregion
    }
}