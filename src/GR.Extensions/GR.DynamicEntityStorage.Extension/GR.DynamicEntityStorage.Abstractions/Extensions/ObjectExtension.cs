using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mapster;
using Newtonsoft.Json;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.DynamicEntityStorage.Abstractions.Helpers;

namespace GR.DynamicEntityStorage.Abstractions.Extensions
{
    public enum MethodName
    {
        GetAllWithInclude,
        GetAllWhitOutInclude,
        GetByIdWithReflection,
        AddWithReflection,
        UpdateWithReflection,
        Delete,
        DeletePermanent,
        Restore,
        GetTableConfigurations,
        AddDataRangeWithReflection,
        Any,
        FirstOrDefault,
        Count
    }

    /// <summary>
    /// Object extension
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// Create object
        /// </summary>
        /// <param name="dynamicObject"></param>
        /// <param name="call"></param>
        /// <param name="types"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static ResultModel<T> Invoke<T>(this DynamicObject dynamicObject, MethodName call, IEnumerable<Type> types = null, IEnumerable<dynamic> parameters = null)
        {
            var result = new ResultModel<T>();
            try
            {
                var method = dynamicObject.Service.GetType().GetMethod(call.ToString())
                    ?.MakeGenericMethod(types?.ToArray() ?? new[] { typeof(object) });

                if (method == null)
                {
                    throw new Exception("Method not supported!");
                }
                var task = (Task)method.Invoke(dynamicObject.Service, parameters?.ToArray());
                Task.WaitAll(task);
                var res = ((dynamic)task).Result;
                result.IsSuccess = res.IsSuccess;
                result.Errors = res.Errors;
                result.Result = res.Result;

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                result.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
                return result;
            }
        }

        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Add<TEntity>(this DynamicObject obj, TEntity model)
        => Task.Run(() =>
        {
            var result = new ResultModel<Guid>();
            if (model == null) return result;
            var target = obj.Type != typeof(TEntity) ? model.ParseDynamicObjectByType(obj.Type) : model;
            var req = obj.Invoke<Guid>(MethodName.AddWithReflection, new List<Type> { target.GetType() },
                new List<object> { target });
            return req;
        });

        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<ResultModel<dynamic>> AddRange<TEntity>(this DynamicObject obj, IEnumerable<TEntity> model)
        => Task.Run(() =>
        {
            var result = new ResultModel();
            if (model == null || !model.Any()) return result;
            //var data = obj.ParseListObject(model);
            var req = obj.Invoke<dynamic>(MethodName.AddDataRangeWithReflection, new List<Type> { obj.Type },
                new List<object> { model });
            return req;
        });

        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static Task<ResultModel<IEnumerable<TEntity>>> GetAllWithInclude<TEntity>(this DynamicObject obj, Func<TEntity, bool> func = null, IEnumerable<Filter> filters = null) where TEntity : class
        => Task.Run(() =>
        {
            var result = new ResultModel<IEnumerable<TEntity>>();
            //var fType = typeof(Func<,>);
            //var genericFunc = fType.MakeGenericType(obj.Type, typeof(bool));

            //var param = func == null ? null : Delegate.CreateDelegate(genericFunc, func.Target, func.Method);
            var req = obj.Invoke<IEnumerable<TEntity>>(MethodName.GetAllWithInclude,
                new List<Type> { obj.Type, obj.Type }, new List<dynamic> { null, filters });

            if (!req.IsSuccess) return result;
            result.IsSuccess = true;
            result.Result = req.Result.Adapt<IEnumerable<TEntity>>() ?? new List<TEntity>();
            result.Result = func != null ? result.Result.Where(func).ToList() : result.Result.ToList();
            return result;
        });


        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static Task<ResultModel<IEnumerable<TEntity>>> GetAll<TEntity>(this DynamicObject obj, Func<TEntity, bool> func = null, IEnumerable<Filter> filters = null) where TEntity : class
        => Task.Run(() =>
        {
            var result = new ResultModel<IEnumerable<TEntity>>();
            var req = obj.Invoke<IEnumerable<TEntity>>(MethodName.GetAllWhitOutInclude,
                new List<Type> { obj.Type, obj.Type }, new List<dynamic> { null, filters });

            if (!req.IsSuccess) return result;
            result.IsSuccess = true;
            result.Result = req.Result.Adapt<IEnumerable<TEntity>>();
            result.Result = func != null ? result.Result?.Where(func).ToList() : result.Result?.ToList();
            return result;
        });

        /// <summary>
        /// Check any
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<TEntity>> GetById<TEntity>(this DynamicObject obj, Guid id)
        => Task.Run(() =>
        {
            var result = new ResultModel<TEntity>();
            if (id == Guid.Empty) return result;
            var req = obj.Invoke<TEntity>(MethodName.GetByIdWithReflection, new List<Type> { obj.Type, obj.Type },
                new List<object> { id });
            return req;
        });

        /// <summary>
        /// Check if any rows
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<ResultModel<bool>> Any(this DynamicObject obj)
       => Task.Run(() =>
       {
           var req = obj.Invoke<bool>(MethodName.Any, new List<Type> { obj.Type },
               new List<object>());
           return req;
       });


        /// <summary>
        /// Get count by filters
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static Task<ResultModel<int>> Count(this DynamicObject obj, Dictionary<string, object> filters)
            => Task.Run(() =>
            {
                var req = obj.Invoke<int>(MethodName.Count, new List<Type> { obj.Type },
                    new List<object> { filters });
                return req;
            });

        /// <summary>
        /// Get By id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task<ResultModel<TEntity>> FirstOrDefault<TEntity>(this DynamicObject obj, Expression<Func<TEntity, bool>> predicate)
            => Task.Run(() =>
            {
                var req = obj.Invoke<TEntity>(MethodName.FirstOrDefault, new List<Type> { obj.Type },
                    new List<object> { predicate });
                return req;
            });

        /// <summary>
        /// Delete item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Delete<TEntity>(this DynamicObject obj, Guid id)
       => Task.Run(() =>
       {
           var result = new ResultModel<Guid>();
           if (id == Guid.Empty) return result;
           var req = obj.Invoke<TEntity>(MethodName.Delete, new List<Type> { obj.Type },
               new List<object> { id });
           if (!req.IsSuccess) return result;
           result.IsSuccess = true;
           result.Result = req.Result.Adapt<Guid>();
           return result;
       });

        /// <summary>
        /// Delete item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> DeletePermanent<TEntity>(this DynamicObject obj, Guid id)
            => Task.Run(() =>
            {
                var result = new ResultModel<Guid>();
                if (id == Guid.Empty) return result;
                var req = obj.Invoke<TEntity>(MethodName.DeletePermanent, new List<Type> { obj.Type },
                    new List<object> { id });
                if (!req.IsSuccess)
                {
                    result.Errors = req.Errors;
                    return result;
                }
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<Guid>();
                return result;
            });

        /// <summary>
        /// Delete item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Restore<TEntity>(this DynamicObject obj, Guid id)
        => Task.Run(() =>
        {
            var result = new ResultModel<Guid>();
            if (id == Guid.Empty) return result;
            var req = obj.Invoke<TEntity>(MethodName.Restore, new List<Type> { obj.Type },
                new List<object> { id });
            if (!req.IsSuccess) return result;
            result.IsSuccess = true;
            result.Result = req.Result.Adapt<Guid>();
            return result;
        });

        /// <summary>
        /// Update item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Update<TEntity>(this DynamicObject obj, TEntity model)
        => Task.Run(() =>
        {
            var result = new ResultModel<Guid>();
            if (model == null) return result;
            var req = obj.Invoke<Guid>(MethodName.UpdateWithReflection, new List<Type> { model.GetType() },
                new List<dynamic> { model });
            return req;
        });

        /// <summary>
        /// Get new instance
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetNewObjectInstance(this DynamicObject obj)
        {
            Arg.NotNull(obj, nameof(DynamicObject));
            return Activator.CreateInstance(obj.Type);
        }

        /// <summary>
        /// Parse dynamic object to new type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="newType"></param>
        /// <returns></returns>
        public static object ParseDynamicObjectByType(this object obj, Type newType)
        {
            Arg.NotNull(obj, newType.Name);
            try
            {
                var serialText = JsonConvert.SerializeObject(obj);
                return JsonConvert.DeserializeObject(serialText, newType);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
    }
}
