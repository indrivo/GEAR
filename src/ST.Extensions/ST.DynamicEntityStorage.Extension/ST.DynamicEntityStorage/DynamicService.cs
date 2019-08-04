using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Enums;
using ST.Audit.Extensions;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.DynamicEntityStorage.Extensions;
using ST.Entities.Controls.Builders;
using ST.Entities.Data;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Constants;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;
using ST.Entities.Abstractions.ViewModels.Table;
using ST.Identity.Abstractions;

namespace ST.DynamicEntityStorage
{
    /// <inheritdoc />
    public class DynamicService<TContext> : IDynamicService where TContext : EntitiesDbContext, IEntityContext
    {
        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly TContext _context;

        /// <summary>
        /// Inject http context
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="httpContextAccessor"></param>
        public DynamicService(TContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get entity info 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        // ReSharper disable once UnusedTupleComponentInReturnValue
        protected virtual (string, bool, ErrorModel, TableModel) GetEntityInfoSchema<TEntity>()
        {
            var typeName = typeof(TEntity).Name;
            return GetEntityInfoSchema(typeName);
        }

        /// <summary>
        /// Get entity info 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        // ReSharper disable once UnusedTupleComponentInReturnValue
        protected virtual (string, bool, ErrorModel, TableModel) GetEntityInfoSchema(string entity)
        {
            var table = _context.Table.FirstOrDefault(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId || x.Name.Equals(entity) && x.IsCommon);
            if (table == null)
                return (null, false, new ErrorModel("entity_not_found", "Entity not found!"), null);

            return (table.EntityType, true, null, table);
        }

        /// <summary>
        /// Get entity info 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns></returns>
        // ReSharper disable once UnusedTupleComponentInReturnValue
        protected virtual async Task<(string, bool, ErrorModel, TableModel)> GetEntityInfoSchemaAsync(string entity)
        {
            var table = await _context.Table.FirstOrDefaultAsync(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId || x.Name.Equals(entity) && x.IsCommon);
            if (table == null)
                return (null, false, new ErrorModel("entity_not_found", "Entity not found!"), null);

            return (table.EntityType, true, null, table);
        }

        /// <summary>
        /// Tenant id
        /// </summary>
        protected virtual Guid? CurrentUserTenantId
        {
            get
            {
                Guid? val = _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value
                              ?.ToGuid() ?? Settings.TenantId;
                var userManager = IoC.Resolve<UserManager<ApplicationUser>>();
                if (val != Guid.Empty) return val;
                var user = userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User).GetAwaiter()
                    .GetResult();
                if (user == null) return null;
                var userClaims = userManager.GetClaimsAsync(user).GetAwaiter().GetResult();
                val = userClaims?.FirstOrDefault(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value))?.Value?.ToGuid();

                return val;
            }
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
            foreach (var item in model)
            {
                item.TenantId = CurrentUserTenantId;
            }

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
            var adapt = model.Adapt<IEnumerable<TOutput>>();
            result.Result = adapt.ToList();
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
            if (!rq.IsSuccess) return default;
            try
            {
                var data = predicate == null ? rq.Result.ToList() : rq.Result.Where(predicate).ToList();
                return new ResultModel<IEnumerable<TOutput>>
                {

                    IsSuccess = true,
                    Result = data
                };
            }
            catch
            {
                return default;
            }
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
            var (schema, state, errorModel, _) = GetEntityInfoSchema<TEntity>();
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
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
            var (schema, state, errorModel, _) = GetEntityInfoSchema(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            result.IsSuccess = true;
            var model = await CreateEntityDefinition<EntityViewModel>(entity, schema);
            model.Values = GetFilters(filters);
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
            var table = _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefault(x => x.Name == entity);
            if (table == null) return listWithoutInclude;
            var fieldReferences = table.TableFields
                .Where(x => x.TableFieldConfigValues.Any(y => y.TableFieldConfig.Code == TableFieldConfigCode.Reference.ForeingTable)).ToList();
            if (!fieldReferences.Any()) return listWithoutInclude;
            foreach (var item in listWithoutInclude.Result)
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
            var (schema, state, errorModel, _) = GetEntityInfoSchema(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            var model = await CreateEntityDefinition<EntityViewModel>(entity, schema);
            model.Values = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {
                        "Id",
                        id
                    }
                }
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
            var table = _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .FirstOrDefault(x => x.Name == entity);
            if (table == null) return obj;
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
            obj.TenantId = CurrentUserTenantId;
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

            var table = await _context.Table.FirstOrDefaultAsync(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId);
            if (table == null) return result;
            table.TableFields = _context.TableFields.Where(x => x.TableId.Equals(table.Id)).ToList();

            result.Result = table.Adapt<TTable>();
            result.IsSuccess = true;

            //result.IsSuccess = true;
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
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return default;
            var count = await Count<TEntity>();
            if (count.IsSuccess)
            {
                return new ResultModel<bool>
                {
                    Result = count.Result > 0,
                    IsSuccess = true
                };
            }
            return default;
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
            var (schema, state, errorModel, _) = GetEntityInfoSchema<TEntity>();
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
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
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetPaginated<TEntity>(ulong page = 1, ulong perPage = 10) where TEntity : BaseModel
        {
            //TODO: Create paginates sql query result
            var result = new ResultModel<IEnumerable<Dictionary<string, object>>>()
            {
                Result = new List<Dictionary<string, object>>()
            };
            var data = await GetAll<TEntity>();
            if (!data.IsSuccess) return result;
            result.Result = data.Result.Skip(((int)page - 1) * (int)perPage)
                .Take((int)perPage).ToList();
            result.IsSuccess = true;
            return result;
        }
        /// <inheritdoc />
        /// <summary>
        /// Implement add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> Add<TEntity>(Dictionary<string, object> model) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();

            var entity = typeof(TEntity);
            if (string.IsNullOrEmpty(entity.Name)) return result;
            var (schema, state, errorModel, _) = GetEntityInfoSchema(entity.Name);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            var table = await CreateEntityDefinition<TEntity, EntityViewModel>(schema);
            var author = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
            //Set default values
            model.SetDefaultValues(table);
            model["Changed"] = DateTime.Now;
            model["Created"] = DateTime.Now;
            model["Author"] = author;
            model["ModifiedBy"] = author;

            var audit = model.GetTrackAuditFromDictionary(typeof(EntitiesDbContext).FullName, CurrentUserTenantId,
                        entity, TrackEventType.Added);
            if (model.ContainsKey("Version"))
            {
                model["Version"] = audit.Version;
            }
            table.Values = new List<Dictionary<string, object>> { model };
            result = _context.Insert(table);
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
            var (schema, state, errorModel, _) = GetEntityInfoSchema(entity);
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
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
            var req = _context.Refresh(table);
            if (!req.IsSuccess || !req.Result) return result;
            result.IsSuccess = true;
            result.Result = Guid.Parse(model["Id"].ToString());
            var audit = model.GetTrackAuditFromDictionary(typeof(EntitiesDbContext).FullName, CurrentUserTenantId,
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
            var (schema, state, errorModel, _) = GetEntityInfoSchema<TEntity>();
            if (!state)
            {
                result.Errors.Add(errorModel);
                return result;
            }
            var table = await CreateEntityDefinition<TEntity, EntityViewModel>(schema);
            table.Values = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {
                        "Id", id
                    }
                }
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
            model["IsDeleted"] = true;
            model["Changed"] = DateTime.Now;
            model["ModifiedBy"] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
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
            model["IsDeleted"] = false;
            model["Changed"] = DateTime.Now;
            model["ModifiedBy"] = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";
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

            model = await ViewModelBuilder.ResolveAsync(_context, model);
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

            model = await ViewModelBuilder.ResolveAsync(_context, model);
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

            model = ViewModelBuilder.Create(_context, model);
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
                Service = new DynamicService<TContext>(_context, _httpContextAccessor)
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
        public virtual async Task<(List<object>, int)> Filter(string entity, string search, string sortOrder, int start, int length, Expression<Func<object, bool>> predicate = null, IEnumerable<ListFilter> filters = null)
            => await Table(entity).Filter(entity, search, sortOrder, start, length, predicate, filters);

        /// <inheritdoc />
        /// <summary>
        /// Create dynamic tables
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="schemaName"></param>
        public async Task CreateDynamicTablesFromInitialConfigurationsFile(Guid tenantId, string schemaName = null)
        {
            var syncronizer = IoC.Resolve<EntitySynchronizer>();
            Arg.NotNull(syncronizer, nameof(EntitySynchronizer));
            var entitiesList = new List<SeedEntity>
            {
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "SysEntities.json")),
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "Configuration/CustomEntities.json")),
                JsonParser.ReadObjectDataFromJsonFile<SeedEntity>(Path.Combine(AppContext.BaseDirectory, "ProfileEntities.json"))
            };

            foreach (var item in entitiesList)
            {
                if (item.SynchronizeTableViewModels == null) continue;
                foreach (var ent in item.SynchronizeTableViewModels)
                {
                    if (!await IoC.Resolve<EntitiesDbContext>().Table.AnyAsync(s => s.Name == ent.Name && s.TenantId == tenantId))
                    {
                        await syncronizer.SynchronizeEntities(ent, tenantId, schemaName);
                    }
                }
            }
        }

        /// <summary>
        /// Create dynamic tables by replicate from system schema 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public async Task CreateDynamicTablesByReplicateSchema(Guid tenantId, string schemaName = null)
        {
            var syncronizer = IoC.Resolve<EntitySynchronizer>();
            Arg.NotNull(syncronizer, nameof(EntitySynchronizer));
            var entities = await _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldType)
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .Where(x => !x.IsCommon && !x.IsPartOfDbContext)
                .ToListAsync();

            foreach (var item in entities)
            {
                var tableConfig = await GetTableConfiguration(item.Id, item);
                if (!tableConfig.IsSuccess) return;
                var entity = tableConfig.Result;
                if (!await IoC.Resolve<EntitiesDbContext>().Table.AnyAsync(s => s.Name == entity.Name && s.TenantId == tenantId))
                {
                    await syncronizer.SynchronizeEntities(entity, tenantId, schemaName);
                }
            }
        }

        /// <summary>
        /// Get table configuration
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="tableModel"></param>
        /// <returns></returns>
        public async Task<ResultModel<SynchronizeTableViewModel>> GetTableConfiguration(Guid tableId, TableModel tableModel = null)
        {
            var result = new ResultModel<SynchronizeTableViewModel>();
            var table = tableModel ?? await _context.Table
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldType)
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldConfigValues)
                .ThenInclude(x => x.TableFieldConfig)
                .Include(x => x.TableFields)
                .ThenInclude(x => x.TableFieldType)
                .FirstOrDefaultAsync(x => x.Id.Equals(tableId));
            if (table == null)
            {
                result.Errors.Add(new ErrorModel("", "Table not found"));
                return result;
            }

            var fields = await GetTableFieldsForBuildMode(table);
            var model = new SynchronizeTableViewModel
            {
                Name = table.Name,
                Description = table.Description,
                IsStaticFromEntityFramework = table.IsPartOfDbContext,
                IsSystem = table.IsSystem,
                Schema = table.EntityType,
                Fields = fields.ToList()
            };
            result.IsSuccess = true;
            result.Result = model;
            return result;
        }

        /// <summary>
        /// Get table field configurations
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<FieldConfigViewModel>> GetTableFieldConfigurations(TableModelField field)
        {
            Arg.NotNull(field, nameof(TableModelField));
            var fieldType = await _context.TableFieldTypes.FirstOrDefaultAsync(x => x.Id == field.TableFieldTypeId);
            var fieldTypeConfig = _context.TableFieldConfigs.Where(x => x.TableFieldTypeId == fieldType.Id).ToList();
            var configFields = field.TableFieldConfigValues
                .Where(z => fieldTypeConfig.FirstOrDefault(x => x.Id == z.TableFieldConfigId) != null)
                .Select(y =>
                {
                    var fTypeConfig = fieldTypeConfig.Single(x => x.Id == y.TableFieldConfigId);
                    
                    return new FieldConfigViewModel
                    {
                        Name = fTypeConfig.Name,
                        Type = fTypeConfig.Type,
                        ConfigId = y.TableFieldConfigId,
                        Description = fTypeConfig.Description,
                        ConfigCode = fTypeConfig.Code,
                        Value = y.Value
                    };
                }).ToList();
            return configFields;
        }

        /// <summary>
        /// Get table fields for builder mode
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<CreateTableFieldViewModel>> GetTableFieldsForBuildMode(TableModel table)
        {
            Arg.NotNull(table, nameof(TableModel));
            var fieldTypes = await _context.TableFieldTypes
                .Include(x => x.TableFieldGroups)
                .ToListAsync();
            var result = new List<CreateTableFieldViewModel>();

            foreach (var field in table.TableFields)
            {
                var groupName = fieldTypes.FirstOrDefault(u => u.DataType.Equals(field.DataType))
                    ?.TableFieldGroups
                    ?.GroupName;
                var configurations = await GetTableFieldConfigurations(field);
                var model = new CreateTableFieldViewModel
                {
                    Id = field.Id,
                    Name = field.Name,
                    Description = field.Description,
                    AllowNull = field.AllowNull,
                    Parameter = groupName,
                    DataType = field.DataType,
                    DisplayName = field.DisplayName,
                    Configurations = configurations.ToList()
                };
                result.Add(model);
            }

            return result;
        }

        /// <summary>
        /// Duplicate tables for schema in database 
        /// </summary>
        /// <param name="schema"></param>
        /// <returns></returns>
        public virtual async Task DuplicateEntitiesForSchema(string schema)
        {
            var tableBuilder = IoC.Resolve<ITablesService>();
            var connection = _context.Database.GetDbConnection().ConnectionString;
            if (!_context.EntityTypes.Any(x => x.MachineName.ToLowerInvariant().Equals(schema.ToLowerInvariant()))) return;
            var entities = await _context.Table
                .Include(x => x.TableFields)
                    .ThenInclude(x => x.TableFieldType)
                .Include(x => x.TableFields)
                    .ThenInclude(x => x.TableFieldConfigValues)
                        .ThenInclude(x => x.TableFieldConfigId)
                .Where(x => !x.IsCommon && !x.IsPartOfDbContext)
                .ToListAsync();

            Parallel.ForEach(entities, async table =>
            {
                table.EntityType = schema;
                var dbTableResponse = tableBuilder.CreateSqlTable(table, connection);
                if (!dbTableResponse.IsSuccess) return;
                var fields = await GetTableFieldsForBuildMode(table);
                Parallel.ForEach(fields, y =>
                {
                    var newFieldResult = tableBuilder.AddFieldSql(y, table.Name, connection, true, schema);
                    if (!newFieldResult.IsSuccess)
                    {
                        Debug.WriteLine(newFieldResult.Errors);
                    }
                });
            });
        }
    }
}
