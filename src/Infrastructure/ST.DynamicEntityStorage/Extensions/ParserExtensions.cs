using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.DynamicEntityStorage.Abstractions.Helpers;
using ST.Entities.Data;
using ST.Entities.Models.Tables;

namespace ST.DynamicEntityStorage.Extensions
{
    public static class ParserExtensions
    {
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
        /// Parse object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="conf">
        /// </param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ParseObject<TObject, TContext>(this ObjectService conf, TObject obj) where TContext : EntitiesDbContext
        {
            return conf.ParseObject(obj);
        }

        /// <summary>
        /// Parse object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="conf"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ParseObject<TObject, TContext>(this DynamicObject conf, TObject obj) where TContext : EntitiesDbContext
        {
            var entity = conf.Object.GetType().Name;
            var res = new ObjectService(entity).ParseObject(obj);
            return res;
        }

        /// <summary>
        /// Parse list object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="conf"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<object> ParseListObject<TObject, TContext>(this DynamicObject conf, IEnumerable<TObject> list) where TContext : EntitiesDbContext
        {
            return list.Select(conf.ParseObject<TObject, TContext>).ToList();
        }
    }
}
