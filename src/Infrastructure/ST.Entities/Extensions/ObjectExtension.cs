using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using ST.BaseBusinessRepository;
using ST.Entities.Models.Tables;
using ST.Entities.Services;

namespace ST.Entities.Extensions
{
    public enum MethodName
    {
        GetAllSystem,
        GetByIdSystem,
        AddSystem,
        UpdateSystem,
        Delete,
        Restore,
        GetTableConfigurations
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
        private static ResultModel<T> Invoke<T>(this DynamicObject dynamicObject, MethodName call, IEnumerable<Type> types = null, IEnumerable<dynamic> parameters = null)
        {
            var result = new ResultModel<T>();
            try
            {
                var method = dynamicObject.DataService.GetType().GetMethod(call.ToString())
                    .MakeGenericMethod(types?.ToArray() ?? new[] { typeof(object) });

                var task = (Task)method.Invoke(dynamicObject.DataService, parameters?.ToArray());
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
               var req = obj.Invoke<Guid>(MethodName.AddSystem, new List<Type> { model.GetType() },
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
        /// <param name="func"></param>
        /// <returns></returns>
        public static Task<ResultModel<IEnumerable<TEntity>>> GetAll<TEntity>(this DynamicObject obj, Func<TEntity, bool> func = null)
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<IEnumerable<TEntity>>();
                var req = obj.Invoke<IEnumerable<TEntity>>(MethodName.GetAllSystem, new List<Type> { obj.Object.GetType(), obj.Object.GetType() });
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<IEnumerable<TEntity>>();
                result.Result = func != null ? result.Result.Where(func).ToList() : result.Result.ToList();
                return result;
            });
        }
        /// <summary>
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
                var req = obj.Invoke<TEntity>(MethodName.GetByIdSystem, new List<Type> { obj.Object.GetType(), obj.Object.GetType() },
                    new List<object> { id });
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<TEntity>();
                return result;
            });
        }
        /// <summary>
        /// Get table configurations
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Task<ResultModel<TableModel>> GetTableConfigurations(this DynamicObject obj)
        {
            return Task.Run(() =>
            {
                var result = new ResultModel<TableModel>();
                var req = obj.Invoke<TableModel>(MethodName.GetTableConfigurations, new List<Type> { obj.Object.GetType() });
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
                var req = obj.Invoke<TEntity>(MethodName.Delete, new List<Type> { obj.Object.GetType() },
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
                var req = obj.Invoke<TEntity>(MethodName.Restore, new List<Type> { obj.Object.GetType() },
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
                var req = obj.Invoke<Guid>(MethodName.UpdateSystem, new List<Type> { obj.Object.GetType() },
                    new List<dynamic> { model });
                if (!req.IsSuccess) return result;
                result.IsSuccess = true;
                result.Result = req.Result.Adapt<Guid>();
                return result;
            });
        }
        /// <summary>
        /// Parse object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="conf"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ParseObject<TObject>(this ObjectService conf, TObject obj)
        {
            return conf.ParseObject(obj);
        }
        /// <summary>
        /// Parse object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="conf"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ParseObject<TObject>(this DynamicObject conf, TObject obj)
        {
            var entity = conf.Object.GetType().Name;
            return new ObjectService(entity).ParseObject(obj);
        }

        /// <summary>
        /// Get Entity Id
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Guid GetValueId(this object obj)
        {
            try
            {
                return Guid.Parse(((dynamic)obj).Id.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Guid.Empty;
        }
    }
}
