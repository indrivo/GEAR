﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions.Helpers;

namespace ST.DynamicEntityStorage.Abstractions.Extensions
{
    public enum MethodName
    {
        GetAllWithInclude,
        GetByIdWithReflection,
        AddWithReflection,
        UpdateWithReflection,
        Delete,
        Restore,
        GetTableConfigurations,
        AddDataRangeWithReflection,
        Any
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
                    .MakeGenericMethod(types?.ToArray() ?? new[] { typeof(object) });

                var task = (Task)method.Invoke(dynamicObject.Service, parameters?.ToArray());
                task.Wait();
                var res = ((dynamic)task).Result;
                result.Result = res.Result;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
        {
            return Task.Run(() =>
           {
               var result = new ResultModel<Guid>();
               if (model == null) return result;
               var req = obj.Invoke<Guid>(MethodName.AddWithReflection, new List<Type> { model.GetType() },
                   new List<object> { model });
               if (!req.IsSuccess) return result;
               result.IsSuccess = true;
               result.Result = req.Result.Adapt<Guid>();
               return result;
           });
        }

        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<ResultModel> AddRange<TEntity>(this DynamicObject obj, IEnumerable<TEntity> model)
        {
            return Task.Run(() =>
            {
                var result = new ResultModel();
                if (model == null || !model.Any()) return result;
                //var data = obj.ParseListObject(model);
                var req = obj.Invoke<dynamic>(MethodName.AddDataRangeWithReflection, new List<Type> { obj.Type },
                    new List<object> { model });
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                return result;
            });
        }

        /// <summary>
        /// Add new item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static Task<ResultModel<IEnumerable<TEntity>>> GetAll<TEntity>(this DynamicObject obj, Func<TEntity, bool> func = null, IEnumerable<Filter> filters = null) where TEntity : class
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<IEnumerable<TEntity>>();
                var fType = typeof(Func<,>);
                //var genericFunc = fType.MakeGenericType(obj.Type, typeof(bool));

                //var param = func == null ? null : Delegate.CreateDelegate(genericFunc, func.Target, func.Method);
                var req = obj.Invoke<IEnumerable<TEntity>>(MethodName.GetAllWithInclude,
                    new List<Type> { obj.Type, obj.Type }, new List<dynamic> { null, filters });

                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<IEnumerable<TEntity>>();
                result.Result = func != null ? result.Result.Where(func).ToList() : result.Result.ToList();
                return result;
            });
        }

        /// <summary>0
        /// Get By id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<TEntity>> GetById<TEntity>(this DynamicObject obj, Guid id)
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<TEntity>();
                if (id == Guid.Empty) return result;
                var req = obj.Invoke<TEntity>(MethodName.GetByIdWithReflection, new List<Type> { obj.Type, obj.Type },
                    new List<object> { id });
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<TEntity>();
                return result;
            });
        }

        /// <summary>
        /// Get By id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<ResultModel<bool>> Any(this DynamicObject obj)
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<bool>();
                var req = obj.Invoke<bool>(MethodName.Any, new List<Type> { obj.Type },
                    new List<object>());
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result;
                return result;
            });
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Delete<TEntity>(this DynamicObject obj, Guid id)
        {
            return Task.Run(() =>
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
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Restore<TEntity>(this DynamicObject obj, Guid id)
        {
            return Task.Run(() =>
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
        }

        /// <summary>
        /// Update item
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="obj"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<ResultModel<Guid>> Update<TEntity>(this DynamicObject obj, TEntity model)
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<Guid>();
                if (model == null) return result;
                var req = obj.Invoke<Guid>(MethodName.UpdateWithReflection, new List<Type> { model.GetType() },
                    new List<dynamic> { model });
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<Guid>();
                return result;
            });
        }
    }
}
