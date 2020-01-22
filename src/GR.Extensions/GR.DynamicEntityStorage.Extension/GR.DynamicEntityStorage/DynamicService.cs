using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions.Enums;
using GR.Audit.Abstractions.Extensions;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.DynamicEntityStorage.Abstractions.Helpers;
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
        /// Get all with adapt to Model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<TOutput>>> GetAllWithInclude<TEntity, TOutput>(Func<TEntity, bool> predicate = null, IEnumerable<Filter> filters = null) where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<TOutput>>();
            var data = await GetAll(FuncToExpression(predicate), filters);
            if (!data.IsSuccess) return result;
            var model = GetObject<TEntity>(data.Result)?.ToList();
            if (model == null) return result;
            foreach (var item in model) item.TenantId = _userManager.CurrentUserTenantId;
            model = (await IncludeReferencesOnList(model)).ToList();
            result.IsSuccess = true;
            var adapt = model.Adapt<IEnumerable<TOutput>>();
            result.Result = adapt.ToList();
            return result;
        }

        /// <summary>
        /// Get all without include
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<TOutput>>> GetAllWhitOutInclude<TEntity, TOutput>(Func<TEntity, bool> predicate = null, IEnumerable<Filter> filters = null) where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<TOutput>>();
            var data = await GetAll(FuncToExpression(predicate), filters);
            if (!data.IsSuccess) return result;
            var model = GetObject<TEntity>(data.Result)?.ToList();
            if (model == null) return result;
            result.IsSuccess = true;
            result.Result = predicate != null
                // ReSharper disable once AssignNullToNotNullAttribute
                ? model.Adapt<IEnumerable<TOutput>>()?.Where(predicate as Func<TOutput, bool>).ToList()
                : model.Adapt<IEnumerable<TOutput>>().ToList();
            return result;
        }

        /// <summary>
        /// Func to expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        private static Expression<Func<T, bool>> FuncToExpression<T>(Func<T, bool> f)
        {
            if (f == null) return null;
            return x => f(x);
        }


        /// <inheritdoc />
        /// <summary>
        /// Get filtered data list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<TOutput>>> GetAll<TEntity, TOutput>(Func<TOutput, bool> predicate = null) where TEntity : BaseModel
        {
            var rq = await GetAllWithInclude<TEntity, TOutput>();
            if (!rq.IsSuccess) return rq;
            rq.Result = predicate == null ? rq.Result.ToList() : rq.Result.Where(predicate).ToList();
            return rq;
        }


        /// <summary>
        /// Load single references
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ICollection<TEntity>> IncludeReferencesOnList<TEntity>(ICollection<TEntity> model) where TEntity : BaseModel
        {
            var referenceFields = typeof(TEntity).GetProperties()
                .Where(x => !x.PropertyType.GetTypeInfo().FullName?.StartsWith("System") ?? false)
                .ToList();
            if (!referenceFields.Any()) return model.ToList();
            foreach (var item in model.ToList())
            {
                foreach (var refField in referenceFields)
                {
                    var refEntity = refField.PropertyType.Name;
                    var refPropName = refField.Name;
                    var refId = item.GetType()
                        .GetProperty(refPropName.Split("Reference")[0])
                        ?.GetValue(item)
                        ?.ToString()
                        .ToGuid();

                    if (refId == null) continue;
                    var table = Table(refEntity);

                    try
                    {
                        var refValue = await table
                            .GetById<object>(refId.Value);
                        if (!refValue.IsSuccess) continue;
                        var refType = item.GetType().GetProperty(refPropName)?.PropertyType;
                        if (refType == null) continue;
                        var newInstance = Activator.CreateInstance(refType);
                        foreach (var prop in newInstance.GetType().GetProperties())
                        {
                            var p = refValue.Result
                                .GetType()
                                .GetProperty(prop.Name);
                            if (p == null) continue;
                            var value = p.GetValue(refValue.Result);

                            prop.SetValue(newInstance, value);
                        }
                        item.GetType().GetProperty(refPropName)?.SetValue(item, newInstance);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return model.ToList();
        }


        /// <inheritdoc />
        /// <summary>
        /// Implement Get all with predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>(Func<Dictionary<string, object>, bool> func) where TEntity : BaseModel
        {
            var data = await GetAll<TEntity>();
            try
            {
                if (data.IsSuccess) data.Result = data.Result.ToList().Where(func).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                data.IsSuccess = false;
            }
            return data;
        }


        /// <inheritdoc />
        /// <summary>
        /// Implementation for Get all
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>(Expression<Func<TEntity, bool>> expression = null, IEnumerable<Filter> filters = null) where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<Dictionary<string, object>>>();
            var entity = typeof(TEntity).Name;

            if (string.IsNullOrEmpty(entity)) return result;
            //if (expression != null)
            //{
            //    var translator = new QueryTranslator();
            //    var wherePredicate = translator.Translate(expression);
            //}
            var (schema, state, errorModel, table) = await GetEntityInfoSchemaAsync<TEntity>();
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }

            if (!await _entityRoleAccessService.HaveReadAccessAsync(table.Id)) return new AccessDeniedResult<IEnumerable<Dictionary<string, object>>>();

            result.IsSuccess = true;
            var model = await CreateEntityDefinition<TEntity, EntityViewModel>(schema);
            model.Values = GetFilters(filters);
            var data = _context.ListEntitiesByParams(model);
            result.Result = data.Result.Values;
            return result;
        }


        /// <inheritdoc />
        /// <summary>
        /// Get all  as dictionary collection
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll(string entity, Expression<Func<Dictionary<string, object>, bool>> expression = null, IEnumerable<Filter> filters = null)
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
            var model = await CreateEntityDefinition<EntityViewModel>(entity, schema);
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
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>>
            GetAllWithIncludeAsDictionaryAsync(string entity, Expression<Func<Dictionary<string, object>, bool>> expression = null, IEnumerable<Filter> filters = null)
        {
            var listWithoutInclude = await GetAll(entity, expression, filters);
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
                var obj = await GetById(tableName, parsedId);
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
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, object>>> GetById<TEntity>(Guid id) where TEntity : BaseModel
        {
            var entity = typeof(TEntity).Name;
            return await GetById(entity, id);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, object>>> GetById(string entity, Guid id)
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
            var model = await CreateEntityDefinition<EntityViewModel>(entity, schema);
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
        public virtual async Task<ResultModel<Dictionary<string, object>>> GetByIdWithInclude(string entity, Guid id)
        {
            var obj = await GetById(entity, id);
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
        /// Get entity by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<TOutput>> GetByIdWithReflection<TEntity, TOutput>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<TOutput>();
            var req = await GetById<TEntity>(id);
            if (!req.IsSuccess) return result;
            var obj = GetObject<TEntity>(req.Result);
            if (obj == null) return result;
            obj.TenantId = _userManager.CurrentUserTenantId;
            var adapt = obj.Adapt<TOutput>();
            result.IsSuccess = true;
            result.Result = adapt;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get first or default
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<TEntity>> FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseModel
        {
            var result = new ResultModel<TEntity>();
            var allCheck = await GetAllWithInclude<TEntity, TEntity>(predicate?.Compile());
            if (!allCheck.IsSuccess) return result;
            result.IsSuccess = true;
            result.Result = allCheck.Result.FirstOrDefault();
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get last element
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<TEntity>> LastOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseModel
        {
            var result = new ResultModel<TEntity>();
            var allCheck = await GetAllWithInclude<TEntity, TEntity>(predicate?.Compile());
            if (!allCheck.IsSuccess) return result;
            result.IsSuccess = true;
            result.Result = allCheck.Result.LastOrDefault();
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get table configuration
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TTable"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<TTable>> GetTableConfigurations<TEntity, TTable>() where TEntity : BaseModel where TTable : class
        {
            var result = new ResultModel<TTable>();
            var entity = typeof(TEntity).Name;

            if (string.IsNullOrEmpty(entity)) return result;

            var table = await _context.Table.FirstOrDefaultAsync(x => x.Name.Equals(entity) && x.TenantId == _userManager.CurrentUserTenantId);
            if (table == null) return result;
            table.TableFields = _context.TableFields.Where(x => x.TableId.Equals(table.Id)).ToList();

            result.Result = table.Adapt<TTable>();
            result.IsSuccess = true;
            return result;
        }
        /// <inheritdoc />
        /// <summary>
        /// Implement Any method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<bool>> Any<TEntity>() where TEntity : BaseModel
        {
            var result = new ResultModel<bool>();
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var count = await Count<TEntity>();
            if (!count.IsSuccess) return result;
            result.Result = count.Result > 0;
            result.IsSuccess = true;
            return result;
        }
        /// <inheritdoc />
        /// <summary>
        /// Count all data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<int>> Count<TEntity>(Dictionary<string, object> filters = null) where TEntity : BaseModel
        {
            var result = new ResultModel<int>()
            {
                Result = 0
            };
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var (schema, state, errorModel, table) = await GetEntityInfoSchemaAsync<TEntity>();
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }

            if (!await _entityRoleAccessService.HaveReadAccessAsync(table.Id)) return new AccessDeniedResult<int>();
            var model = await CreateEntityDefinition<TEntity, EntityViewModel>(schema);
            var count = _context.GetCount(model, filters);
            if (!count.IsSuccess) return result;
            result.IsSuccess = count.IsSuccess;
            result.Result = count.Result;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get paginated result
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="queryString"></param>
        /// <param name="filters"></param>
        /// <param name="orderDirection"></param>
        /// <param name="loadIncludes"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<PaginationResponseViewModel>> GetPaginatedResultAsync<TEntity>(uint page = 1, uint perPage = 10, string queryString = null,
            IEnumerable<Filter> filters = null, Dictionary<string, EntityOrderDirection> orderDirection = null, bool loadIncludes = true) where TEntity : BaseModel
        => await GetPaginatedResultAsync(typeof(TEntity).Name, page, perPage, queryString, filters, orderDirection, loadIncludes);

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

            var model = await CreateEntityDefinition<EntityViewModel>(entity, schema);
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
                        var obj = identifier != null ? await GetById(tableName.Value, identifier.Value) : null;
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
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <param name="dbSchema"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> Add<TEntity>(Dictionary<string, object> model, string dbSchema = null) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();

            var entity = typeof(TEntity);
            if (string.IsNullOrEmpty(entity.Name)) return result;
            var (schema, state, errorModel, dto) = await GetEntityInfoSchemaAsync(entity.Name);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveAccessAsync(dto.Id, EntityAccessType.Write)) return new AccessDeniedResult<Guid>();
            var table = await CreateEntityDefinition<TEntity, EntityViewModel>(dbSchema ?? schema);
            var author = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            //Set default values
            model.SetDefaultValues(table);
            model["Changed"] = DateTime.Now;
            model["Created"] = DateTime.Now;
            model["Author"] = author;
            model["ModifiedBy"] = author;

            var audit = model.GetTrackAuditFromDictionary(typeof(EntitiesDbContext).FullName, _userManager.CurrentUserTenantId,
                        entity, TrackEventType.Added);
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
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> AddWithReflection<TEntity>(TEntity model) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            if (model == null) return result;
            var data = GetDictionary(model);
            if (data.Any())
            {
                return await Add<TEntity>(data);
            }
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Add range
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IList<(TEntity, Guid)>>> AddDataRangeWithReflection<TEntity>(IEnumerable<TEntity> data) where TEntity : BaseModel
        {
            var result = new ResultModel<IList<(TEntity, Guid)>>
            {
                Result = new List<(TEntity, Guid)>()
            };
            foreach (var item in data)
            {
                var rq = await AddWithReflection(item);
                result.Result.Add((item, rq.Result));
            }
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Implement add range
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Guid>>> AddRange<TEntity>(IEnumerable<Dictionary<string, object>> items) where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<Guid>>();
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var local = new List<Guid>();
            foreach (var item in items)
            {
                var action = await Add<TEntity>(item);
                local.Add(action.IsSuccess ? action.Result : Guid.Empty);
            }

            result.Result = local;
            result.IsSuccess = true;
            return result;
        }
        /// <inheritdoc />
        /// <summary>
        /// Update item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> Update<TEntity>(Dictionary<string, object> model) where TEntity : BaseModel
        {
            var entity = typeof(TEntity).Name;
            return await Update(entity, model);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> Update(string entity, Dictionary<string, object> model)
        {
            var result = new ResultModel<Guid>();
            var (schema, state, errorModel, dto) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveAccessAsync(dto.Id, EntityAccessType.Update)) return new AccessDeniedResult<Guid>();
            var table = await CreateEntityDefinition<EntityViewModel>(entity, schema);
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
        /// Update item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> UpdateWithReflection<TEntity>(TEntity model) where TEntity : BaseModel
        {
            var dic = GetDictionary(model);
            return await Update<TEntity>(dic);
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete permanent
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> DeletePermanent<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var (schema, state, errorModel, dto) = await GetEntityInfoSchemaAsync<TEntity>();
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            if (!await _entityRoleAccessService.HaveAccessAsync(dto.Id, EntityAccessType.DeletePermanent)) return new AccessDeniedResult<Guid>();
            var table = await CreateEntityDefinition<TEntity, EntityViewModel>(schema);
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
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> Delete<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var item = await GetById<TEntity>(id);
            if (!item.IsSuccess) return result;
            var model = item.Result;
            model[nameof(BaseModel.IsDeleted)] = true;
            model[nameof(BaseModel.Changed)] = DateTime.Now;
            model[nameof(BaseModel.ModifiedBy)] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            result = await Update<TEntity>(model);
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Restore item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> Restore<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var item = await GetById<TEntity>(id);
            if (!item.IsSuccess) return result;
            var model = item.Result;
            model[nameof(BaseModel.IsDeleted)] = false;
            model[nameof(BaseModel.Changed)] = DateTime.Now;
            model[nameof(BaseModel.ModifiedBy)] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            result = await Update<TEntity>(model);
            return result;
        }



        /// <inheritdoc />
        /// <summary>
        /// Check if exists
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> Exists<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel();
            var item = await GetByIdWithReflection<TEntity, TEntity>(id);
            result.IsSuccess = item.IsSuccess;
            return result;
        }


        /// <inheritdoc />
        /// <summary>
        /// Implement create entity model with base model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public virtual async Task<TViewModel> CreateEntityDefinition<TEntity, TViewModel>(string tableSchema) where TEntity : BaseModel where TViewModel : EntityViewModel
        {
            var model = new EntityViewModel
            {
                TableName = typeof(TEntity).Name,
                TableSchema = tableSchema,
                Fields = new List<EntityFieldsViewModel>()
            };

            model = await ViewModelBuilderFactory.ResolveAsync(_context, model);
            return (TViewModel)model;
        }

        public virtual async Task<TViewModel> CreateEntityDefinition<TViewModel>(string entityName, string tableSchema) where TViewModel : EntityViewModel
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


        /// <inheritdoc />
        /// <summary>
        /// Implement create entity model without base model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        public virtual async Task<TViewModel> CreateWithoutBaseModel<TEntity, TViewModel>() where TEntity : BaseModel
            where TViewModel : EntityViewModel
        {
            var entity = typeof(TEntity).Name;
            var (schema, state, _, _) = await GetEntityInfoSchemaAsync(entity);
            if (!state)
            {
                return null;
            }
            var model = new EntityViewModel
            {
                TableName = entity,
                TableSchema = schema,
                Fields = new List<EntityFieldsViewModel>()
            };

            model = await ViewModelBuilderFactory.ResolveAsync(_context, model);
            return (TViewModel)model;
        }



        /// <inheritdoc />
        /// <summary>
        /// Get T object from dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public virtual T GetObject<T>(Dictionary<string, object> dict)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);
            if (dict == null) return (T)obj;
            foreach (var item in dict)
            {
                if (item.Value.Equals(DBNull.Value)) continue;
                try
                {
                    var fieldType = type.GetProperties().FirstOrDefault(x => x.Name.Equals(item.Key))?.PropertyType;

                    switch (fieldType?.Name)
                    {
                        case "Guid":
                            {
                                if (item.Value != null)
                                    type.GetProperty(item.Key).SetValue(obj, item.Value);
                            }
                            break;
                        case "String":
                            {
                                type.GetProperty(item.Key).SetValue(obj, item.Value?.ToString());
                            }
                            break;
                        case "Int32":
                            {
                                if (!string.IsNullOrEmpty(item.Value?.ToString()))
                                {
                                    type.GetProperty(item.Key).SetValue(obj, Convert.ToInt32(item.Value.ToString()));
                                }
                            }
                            break;
                        case "Double":
                            {
                                if (!string.IsNullOrEmpty(item.Value?.ToString()))
                                {
                                    type.GetProperty(item.Key).SetValue(obj, Convert.ToDouble(item.Value.ToString()));
                                }
                            }
                            break;
                        case "Decimal":
                            {
                                if (!string.IsNullOrEmpty(item.Value?.ToString()))
                                {
                                    type.GetProperty(item.Key).SetValue(obj, Convert.ToDecimal(item.Value.ToString()));
                                }
                            }
                            break;
                        case "Nullable`1":
                            {
                                if (fieldType.IsGenericType)
                                {
                                    if (fieldType.GenericTypeArguments.Any())
                                    {
                                        var subType = fieldType.GenericTypeArguments[0].Name;
                                        switch (subType)
                                        {

                                            case "Guid":
                                                {
                                                    type.GetProperty(item.Key).SetValue(obj,
                                                        DBNull.Value.Equals(item.Value) ? default(Nullable)
                                                        : item.Value);
                                                }
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    type.GetProperty(item.Key).SetValue(obj, default(Nullable));
                                }
                            }
                            break;
                        default:
                            type?.GetProperty(item.Key)?.SetValue(obj, item.Value);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return (T)obj;
        }


        /// <inheritdoc />
        /// <summary>
        /// Get T object from IEnumerable dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionaries"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetObject<T>(IEnumerable<Dictionary<string, object>> dictionaries)
        {
            return dictionaries.Select(GetObject<T>).ToList();
        }


        /// <inheritdoc />
        /// <summary>
        /// Implement from object to dictionary
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual Dictionary<string, object> GetDictionary<TEntity>(TEntity model) => ObjectService.GetDictionary(model);

        /// <inheritdoc />
        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual DynamicObject Table(string tableName)
            => new ObjectService(tableName)
                .ResolveAsync(_context, _httpContextAccessor)
                .GetAwaiter()
                .GetResult();

        /// <inheritdoc />
        /// <summary>
        /// Create table
        /// </summary>
        /// <returns></returns>
        public virtual DynamicObject Table<TEntity>() where TEntity : BaseModel
            => new DynamicObject
            {
                Type = typeof(TEntity),
                Service = IoC.Resolve<IDynamicService>()
            };

        /// <inheritdoc />
        /// <summary>
        /// Register in memory
        /// </summary>
        /// <returns></returns>
        public virtual async Task RegisterInMemoryDynamicTypesAsync()
        {
            var context = IoC.Resolve<IEntityContext>();
            var tables = await context.Table.Where(x => !x.IsPartOfDbContext).ToListAsync();
            foreach (var table in tables)
            {
                await new ObjectService(table.Name).ResolveAsync(_context, _httpContextAccessor);
            }
        }


        /// <inheritdoc />
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
        public virtual async Task<(List<T>, int)> Filter<T>(string search, string sortOrder, int start, int length, Expression<Func<T, bool>> predicate = null) where T : BaseModel
            => await Table<T>().Filter(search, sortOrder, start, length, predicate);


        /// <inheritdoc />
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
        public virtual async Task<(List<object>, int)> Filter(string entity, string search, string sortOrder, int start, int length, Expression<Func<object, bool>> predicate = null, IEnumerable<Filter> filters = null)
            => await Table(entity).Filter(entity, search, sortOrder, start, length, predicate, filters);


        #region Helpers

        /// <summary>
        /// Get entity info 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        // ReSharper disable once UnusedTupleComponentInReturnValue
        protected virtual async Task<(string, bool, ErrorModel, TableModel)> GetEntityInfoSchemaAsync<TEntity>()
        {
            var typeName = typeof(TEntity).Name;
            return await GetEntityInfoSchemaAsync(typeName);
        }

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