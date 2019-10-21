using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.DynamicEntityStorage.Abstractions.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Data;

namespace GR.DynamicEntityStorage.Extensions
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
                var req = obj.Invoke<TableModel>(MethodName.GetTableConfigurations, new List<Type> { obj.Type });
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
            var entity = conf.Type.Name;
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
