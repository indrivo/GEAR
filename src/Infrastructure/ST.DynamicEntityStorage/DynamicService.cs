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
using ST.Audit.Enums;
using ST.Audit.Extensions;
using ST.BaseBusinessRepository;
using ST.BaseRepository;
using ST.DynamicEntityStorage.Abstractions;
using ST.DynamicEntityStorage.Extensions;
using ST.DynamicEntityStorage.Utils;
using ST.Entities.Controls.Builders;
using ST.Entities.Data;
using ST.Entities.Models.Tables;
using ST.Entities.ViewModels.DynamicEntities;

namespace ST.DynamicEntityStorage
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DynamicService<TContext> : IDynamicService where TContext : EntitiesDbContext
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
        /// default tenant id
        /// </summary>
        public static Guid? TenantId { get; set; }

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
        /// Tenant id
        /// </summary>
        private Guid? CurrentUserTenantId
        {
            get
            {
                return _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value
                           ?.ToGuid() ?? TenantId;
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Get all with adapt to Model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<TOutput>>> GetAllWithInclude<TEntity, TOutput>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<TOutput>>();
            var data = await GetAll(predicate);
            if (!data.IsSuccess) return result;
            var model = GetObject<TEntity>(data.Result);
            if (model == null) return result;
            model = await IncludeReferencesOnList(model.ToList());
            result.IsSuccess = true;
            var adapt = model.Adapt<IEnumerable<TOutput>>();
            result.Result = adapt.ToList();
            return result;
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
                .Where(x => !x.PropertyType.GetTypeInfo().FullName.StartsWith("System"))
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
                        ?.ToGuid();

                    if (refId == null) continue;
                    var table = Table(refEntity);

                    try
                    {
                        var refValue = await table
                            .GetById<object>(refId.Value);
                        if (!refValue.IsSuccess) continue;
                        var refType = item.GetType().GetProperty(refPropName).PropertyType;
                        var newInstance = Activator.CreateInstance(refType);
                        foreach (var prop in newInstance.GetType().GetProperties())
                        {
                            var p = refValue.Result
                                .GetType()
                                .GetProperty(prop.Name);
                            var value = p.GetValue(refValue.Result);

                            prop.SetValue(newInstance, value);
                        }
                        item.GetType().GetProperty(refPropName).SetValue(item, newInstance);
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
        public virtual async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>(Expression<Func<TEntity, bool>> expression = null) where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<Dictionary<string, object>>>();
            var entity = typeof(TEntity).Name;

            if (string.IsNullOrEmpty(entity)) return result;
            if (expression != null)
            {
                var translator = new QueryTranslator();
                var wherePredicate = translator.Translate(expression);
            }
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId)?.EntityType;
            result.IsSuccess = true;
            var model = await CreateEntityDefinition<TEntity>(schema);
            model.Includes = new List<EntityViewModel>();
            model.Values = new List<Dictionary<string, object>>();
            var data = _context.ListEntitiesByParams(model);
            result.Result = data.Result.Values;
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
            var result = new ResultModel<Dictionary<string, object>>();
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId)?.EntityType;
            var model = await CreateEntityDefinition<TEntity>(schema);
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
        /// <inheritdoc />
        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<TOutput>> GetByIdSystem<TEntity, TOutput>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<TOutput>();
            var req = await GetById<TEntity>(id);
            if (!req.IsSuccess) return result;
            var obj = GetObject<TEntity>(req.Result);
            if (obj == null) return result;
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
            var allCheck = await GetAllWithInclude<TEntity, TEntity>(predicate);
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
            var allCheck = await GetAllWithInclude<TEntity, TEntity>(predicate);
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
        /// <returns></returns>
        public virtual Task<ResultModel<TableModel>> GetTableConfigurations<TEntity>() where TEntity : BaseModel
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<TableModel>();
                var entity = typeof(TEntity).Name;

                if (string.IsNullOrEmpty(entity)) return result;

                var table = _context.Table.FirstOrDefault(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId);
                if (table != null)
                {
                    table.TableFields = _context.TableFields.Where(x => x.TableId.Equals(table.Id)).ToList();

                    result.Result = table;
                    result.IsSuccess = true;
                }

                //result.IsSuccess = true;
                return result;
            });
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
        public virtual async Task<ResultModel<int>> Count<TEntity>() where TEntity : BaseModel
        {
            var result = new ResultModel<int>()
            {
                Result = 0
            };
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId)?.EntityType;
            var model = await CreateEntityDefinition<TEntity>(schema);
            model.Values = new List<Dictionary<string, object>>();
            var count = _context.GetCount(model);
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
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity.Name) && x.TenantId == CurrentUserTenantId)?.EntityType;
            var table = await CreateEntityDefinition<TEntity>(schema);
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
        public virtual async Task<ResultModel<Guid>> AddSystem<TEntity>(TEntity model) where TEntity : BaseModel
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
        public virtual async Task<ResultModel<IList<(TEntity, Guid)>>> AddDataRange<TEntity>(IEnumerable<TEntity> data) where TEntity : BaseModel
        {
            var result = new ResultModel<IList<(TEntity, Guid)>>
            {
                Result = new List<(TEntity, Guid)>()
            };
            foreach (var item in data)
            {
                var rq = await AddSystem(item);
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
            var result = new ResultModel<Guid>();
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(typeof(TEntity).Name) && x.TenantId == CurrentUserTenantId)?.EntityType;
            var table = await CreateEntityDefinition<TEntity>(schema);
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
                typeof(TEntity), TrackEventType.Updated);

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
        public virtual async Task<ResultModel<Guid>> UpdateSystem<TEntity>(TEntity model) where TEntity : BaseModel
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
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(typeof(TEntity).Name) && x.TenantId == CurrentUserTenantId)?.EntityType;
            var table = await CreateEntityDefinition<TEntity>(schema);
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
            if (!res.Result) return result;
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
            var item = await GetByIdSystem<TEntity, TEntity>(id);
            result.IsSuccess = item.IsSuccess;
            return result;
        }


        /// <inheritdoc />
        /// <summary>
        /// Implement create entity model with base model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual async Task<EntityViewModel> CreateEntityDefinition<TEntity>(string tableSchema) where TEntity : BaseModel
        {
            var model = new EntityViewModel
            {
                TableName = typeof(TEntity).Name,
                TableSchema = tableSchema,
                Fields = new List<EntityFieldsViewModel>()
            };

            model = await ViewModelBuilder.ResolveAsync(_context, model);
            return model;
        }

        public virtual async Task<EntityViewModel> CreateEntityDefinition(string entityName, string tableSchema)
        {
            var model = new EntityViewModel
            {
                TableName = entityName,
                TableSchema = tableSchema,
                Fields = new List<EntityFieldsViewModel>()
            };

            model = await ViewModelBuilder.ResolveAsync(_context, model);
            return model;
        }


        /// <inheritdoc />
        /// <summary>
        /// Implement create entity model without base model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual Task<EntityViewModel> CreateWithoutBaseModel<TEntity>() where TEntity : BaseModel
            => Task.Run(() =>
            {
                var entity = typeof(TEntity).Name;
                var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity) && x.TenantId == CurrentUserTenantId)?.EntityType;
                var model = new EntityViewModel
                {
                    TableName = entity,
                    TableSchema = schema,
                    Fields = new List<EntityFieldsViewModel>()
                };

                model = ViewModelBuilder.Create(_context, model);
                return model;
            });



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
            foreach (var (key, value) in dict)
            {
                if (value.Equals(DBNull.Value)) continue;
                try
                {
                    var fieldType = type.GetProperties().FirstOrDefault(x => x.Name.Equals(key))?.PropertyType;

                    switch (fieldType?.Name)
                    {
                        case "Guid":
                            {
                                if (value != null)
                                    type.GetProperty(key).SetValue(obj, value);
                            }
                            break;
                        case "String":
                            {
                                type.GetProperty(key).SetValue(obj, value?.ToString());
                            }
                            break;
                        case "Int32":
                            {
                                if (!string.IsNullOrEmpty(value?.ToString()))
                                {
                                    type.GetProperty(key).SetValue(obj, Convert.ToInt32(value.ToString()));
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
                                                    type.GetProperty(key).SetValue(obj,
                                                        DBNull.Value.Equals(value) ? default(Nullable)
                                                        : value);
                                                }
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    type.GetProperty(key).SetValue(obj, default(Nullable));
                                }
                            }
                            break;
                        default:
                            type.GetProperty(key).SetValue(obj, value);
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
            => new ObjectService(tableName).Resolve(_context, _httpContextAccessor);

        /// <inheritdoc />
        /// <summary>
        /// Create table
        /// </summary>
        /// <returns></returns>
        public virtual DynamicObject Table<TEntity>() where TEntity : BaseModel
            => new DynamicObject
            {
                Object = Activator.CreateInstance(typeof(TEntity)),
                Service = new DynamicService<TContext>(_context, _httpContextAccessor)
            };

        /// <summary>
        /// Register in memory
        /// </summary>
        /// <returns></returns>
        public virtual async Task RegisterInMemoryDynamicTypes()
        {
            var tables = await _context.Table.ToListAsync();
            foreach (var table in tables)
            {
                new ObjectService(table.Name).Resolve(_context, _httpContextAccessor);
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
        /// <returns></returns>
        public virtual async Task<(List<object>, int)> Filter(string entity, string search, string sortOrder, int start, int length, Expression<Func<object, bool>> predicate = null)
            => await Table(entity).Filter(entity, search, sortOrder, start, length, predicate);
    }
}
