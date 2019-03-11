using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mapster;
using ST.BaseBusinessRepository;
using ST.BaseRepository;
using ST.Entities.Controls.Builders;
using ST.Entities.Data;
using ST.Entities.Models.Tables;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Entities.Services.Abstraction;
using ST.Entities.Extensions;
using ST.Entities.Utils;
using System.Diagnostics;

namespace ST.Entities.Services
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DynamicEntityDataService : IDynamicEntityDataService
    {
        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly EntitiesDbContext _context;
        /// <summary>
        /// Store table configurations
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static readonly HashSet<KeyValuePair<string, TableModel>> Store = new HashSet<KeyValuePair<string, TableModel>>();
        /// <summary>
        /// Check if entity is sync
        /// </summary>
        public static HashSet<KeyValuePair<string, bool>> TableSync = new HashSet<KeyValuePair<string, bool>>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public DynamicEntityDataService(EntitiesDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all with adapt to Model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<TOutput>>> GetAllSystem<TEntity, TOutput>() where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<TOutput>>();
            var data = await GetAll<TEntity>();
            if (!data.IsSuccess) return result;
            var model = GetObject<TEntity>(data.Result);
            if (model == null) return result;
            result.IsSuccess = true;
            var adapt = model.Adapt<IEnumerable<TOutput>>();
            result.Result = adapt.ToList();
            return result;
        }

        /// <summary>
        /// Get filtered data list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<TOutput>>> GetAll<TEntity, TOutput>(Func<TOutput, bool> predicate = null) where TEntity : BaseModel
        {
            var rq = await GetAllSystem<TEntity, TOutput>();
            if (!rq.IsSuccess) return default;
            try
            {
                var data = rq.Result.Where(predicate).ToList();
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

        /// <inheritdoc />
        /// <summary>
        /// Implement Get all with predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>(Func<Dictionary<string, object>, bool> func) where TEntity : BaseModel
        {
            var data = await GetAll<TEntity>();
            try
            {
                if (data.IsSuccess) data.Result = data.Result.ToList().Where(func).ToList();
            }
            catch
            {
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
        public async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetAll<TEntity>() where TEntity : BaseModel
        {
            var result = new ResultModel<IEnumerable<Dictionary<string, object>>>();
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity))?.EntityType;
            result.IsSuccess = true;
            var model = await Create<TEntity>(schema);
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
        public async Task<ResultModel<Dictionary<string, object>>> GetById<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Dictionary<string, object>>();
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity))?.EntityType;
            var model = await Create<TEntity>(schema);
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
        public async Task<ResultModel<TOutput>> GetByIdSystem<TEntity, TOutput>(Guid id) where TEntity : BaseModel
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

        /// <summary>
        /// Get first or default
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task<ResultModel<TEntity>> FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseModel
        {
            var result = new ResultModel<TEntity>();
            var translator = new QueryTranslator();
            //TODO: Parse NodeType.Constant of expression for get sql where query
            //var where = translator.Translate(predicate);
            var allCheck = await GetAll<TEntity, TEntity>(predicate?.Compile());
            if (allCheck.IsSuccess)
            {
                result.IsSuccess = true;
                result.Result = allCheck.Result.FirstOrDefault();
            }
            return result;
        }

        /// <summary>
        /// Get last element
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<ResultModel<TEntity>> LastOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : BaseModel
        {
            var result = new ResultModel<TEntity>();
            var translator = new QueryTranslator();
            //TODO: Parse NodeType.Constant of expression for get sql where query
            var allCheck = await GetAll<TEntity, TEntity>(predicate?.Compile());
            if (allCheck.IsSuccess)
            {
                result.IsSuccess = true;
                result.Result = allCheck.Result.LastOrDefault();
            }
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get table configuration
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public Task<ResultModel<TableModel>> GetTableConfigurations<TEntity>() where TEntity : BaseModel
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<TableModel>();
                var entity = typeof(TEntity).Name;

                //var search = Store.FirstOrDefault(x => x.Key.Equals(entity));
                if (string.IsNullOrEmpty(entity)) return result;
                //if (string.IsNullOrEmpty(search.Key))
                //{
                //    var table = _context.Table.FirstOrDefault(x => x.Name.Equals(entity));
                //    if (table != null)
                //    {
                //        table.TableFields = _context.TableFields.Where(x => x.TableId.Equals(table.Id)).ToList();

                //        result.Result = table;
                //        Store.Add(new KeyValuePair<string, TableModel>(entity, table));
                //    }
                //}
                //else
                //{
                //    result.Result = search.Value;
                //}

                var table = _context.Table.FirstOrDefault(x => x.Name.Equals(entity));
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
        public async Task<bool> Any<TEntity>() where TEntity : BaseModel
        {
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return false;
            var count = await Count<TEntity>();
            if (count.IsSuccess)
            {
                return count.Result > 0;
            }
            return false;
        }
        /// <inheritdoc />
        /// <summary>
        /// Count all data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task<ResultModel<int>> Count<TEntity>() where TEntity : BaseModel
        {
            var result = new ResultModel<int>()
            {
                Result = 0
            };
            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity))?.EntityType;
            var model = await Create<TEntity>(schema);
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
        public async Task<ResultModel<IEnumerable<Dictionary<string, object>>>> GetPaginated<TEntity>(ulong page = 1, ulong perPage = 10) where TEntity : BaseModel
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
        public async Task<ResultModel<Guid>> Add<TEntity>(Dictionary<string, object> model) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();


            var entity = typeof(TEntity).Name;
            if (string.IsNullOrEmpty(entity)) return result;
            var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity))?.EntityType;
            var table = await Create<TEntity>(schema);
            //Set default values
            model.SetDefaultValues(table);

            table.Values = new List<Dictionary<string, object>> { model };
            return _context.Insert(table);
        }

        /// <inheritdoc />
        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> AddSystem<TEntity>(TEntity model) where TEntity : BaseModel
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

        /// <summary>
        /// Add range
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ResultModel<(TEntity, Guid)>>> AddRange<TEntity>(IEnumerable<TEntity> data) where TEntity : BaseModel
        {
            var result = new List<ResultModel<(TEntity, Guid)>>();
            foreach (var item in data)
            {
                var rq = await AddSystem(item);
                result.Add(new ResultModel<(TEntity, Guid)>
                {
                    IsSuccess = rq.IsSuccess,
                    Result = (item, rq.Result)
                });
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
        public async Task<ResultModel<IEnumerable<Guid>>> AddRange<TEntity>(IEnumerable<Dictionary<string, object>> items) where TEntity : BaseModel
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
        public async Task<ResultModel<Guid>> Update<TEntity>(Dictionary<string, object> model) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var schema = _context.Table.Where(x => x.Name.ToLower().Equals(typeof(TEntity).Name.ToLower())).FirstOrDefault()?.EntityType;
            //var schema = _context.Table.FirstOrDefault(x => x.Name.ToLower().Equals(typeof(TEntity).Name))?.EntityType;
            var table = await Create<TEntity>(schema);

            table.Values = new List<Dictionary<string, object>> { model };
            var req = _context.Refresh(table);
            if (!req.IsSuccess || !req.Result) return result;
            result.IsSuccess = true;
            result.Result = Guid.Parse(model["Id"].ToString());
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> UpdateSystem<TEntity>(TEntity model) where TEntity : BaseModel
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
        public async Task<ResultModel<Guid>> DeletePermanent<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var schema = _context.Table.FirstOrDefault(x => x.Name.ToLower().Equals(typeof(TEntity).Name))?.EntityType;
            var table = await Create<TEntity>(schema);
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
            if (res.Result)
            {
                result.IsSuccess = true;
                result.Result = id;
            }
            return result;
        }
        /// <inheritdoc />
        /// <summary>
        /// Change status to is deleted
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> Delete<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var item = await GetById<TEntity>(id);
            if (!item.IsSuccess) return result;
            var model = item.Result;
            model["IsDeleted"] = true;
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
        public async Task<ResultModel<Guid>> Restore<TEntity>(Guid id) where TEntity : BaseModel
        {
            var result = new ResultModel<Guid>();
            var item = await GetById<TEntity>(id);
            if (!item.IsSuccess) return result;
            var model = item.Result;
            model["IsDeleted"] = false;
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
        public async Task<ResultModel> Exists<TEntity>(Guid id) where TEntity : BaseModel
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
        public async Task<EntityViewModel> Create<TEntity>(string tableSchema) where TEntity : BaseModel
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
        /// <inheritdoc />
        /// <summary>
        /// Implement create entity model without base model
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public Task<EntityViewModel> CreateWithoutBaseModel<TEntity>() where TEntity : BaseModel
            => Task.Run(() =>
            {
                var entity = typeof(TEntity).Name;
                var schema = _context.Table.FirstOrDefault(x => x.Name.Equals(entity))?.EntityType;
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
        public T GetObject<T>(Dictionary<string, object> dict)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);
            if (dict == null) return (T)obj;
            foreach (var kv in dict)
            {
                try
                {
                    var fieldType = type.GetProperties().FirstOrDefault(x => x.Name.Equals(kv.Key))?.PropertyType;

                    switch (fieldType.Name)
                    {
                        case "Guid":
                            {
                                type.GetProperty(kv.Key).SetValue(obj, kv.Value);
                            }
                            break;
                        case "String":
                            {
                                type.GetProperty(kv.Key).SetValue(obj, kv.Value.ToString());
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
                                                    type.GetProperty(kv.Key).SetValue(obj,
                                                        DBNull.Value.Equals(kv.Value) ? default(Nullable)
                                                        : kv.Value);
                                                }
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    type.GetProperty(kv.Key).SetValue(obj, default(Nullable));
                                }
                            }
                            break;
                        default:
                            type.GetProperty(kv.Key).SetValue(obj, kv.Value);
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
        public IEnumerable<T> GetObject<T>(IEnumerable<Dictionary<string, object>> dictionaries)
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
        public Dictionary<string, object> GetDictionary<TEntity>(TEntity model) => ObjectService.GetDictionary(model);

        /// <inheritdoc />
        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DynamicObject Table(string tableName)
            => new ObjectService(tableName).Resolve(_context);

        /// <inheritdoc />
        /// <summary>
        /// Create table
        /// </summary>
        /// <returns></returns>
        public DynamicObject Table<TEntity>() where TEntity : BaseModel
            => new DynamicObject
            {
                Object = Activator.CreateInstance(typeof(TEntity)),
                DataService = new DynamicEntityDataService(_context)
            };

        /// <summary>
        /// Filter list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<(List<T>, int)> Filter<T>(string search, string sortOrder, int start, int length, Func<T, bool> predicate = null) where T : BaseModel
            => await Table<T>().Filter<T>(search, sortOrder, start, length, predicate);


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
        public async Task<(List<object>, int)> Filter(string entity, string search, string sortOrder, int start, int length, Func<object, bool> predicate = null)
            => await Table(entity).Filter(entity, search, sortOrder, start, length, predicate);

        /// <inheritdoc />
        /// <summary>
        /// Table builder
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public ObjectService TableBuild(string tableName) => new ObjectService(tableName);
    }
}
