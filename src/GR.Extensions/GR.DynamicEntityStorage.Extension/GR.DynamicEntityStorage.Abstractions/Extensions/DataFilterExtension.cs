using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Core.Helpers.Filters;
using GR.DynamicEntityStorage.Abstractions.Helpers;

namespace GR.DynamicEntityStorage.Abstractions.Extensions
{
    public static class DataFilterExtension
    {
        /// <summary>
        /// Filter predicate
        /// </summary>
        private static Func<(object, string), bool> FilterPredicate { get; } = delegate (ValueTuple<object, string> data)
        {
            if (string.IsNullOrEmpty(data.Item2)) return true;
            try
            {
                var props = data.Item1.GetType().GetProperties();
                return props.Any(prop =>
                {
                    var val = prop.GetValue(data.Item1);
                    return val is string && val.ToString().ToLower().Contains(data.Item2.ToLower());
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        };

        /// <summary>
        /// Custom filter for js data-table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="dbSearch"></param>
        /// <returns></returns>
        public static List<T> Filter<T>(this DbContext context, string search, string sortOrder, int start, int length,
            out int totalCount, Func<T, bool> dbSearch = null) where T : class
        {
            var rh = dbSearch != null ? context.Set<T>().Where(dbSearch).ToList() : context.Set<T>().ToList();

            var result = rh.ToList<dynamic>().Where(p => FilterPredicate((p, search))).ToList();

            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();

            result = result.Order(sortOrder);

            return result.Adapt<List<T>>();
        }

        /// <summary>
        /// Custom filter for js data-table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="dbSearch"></param>
        /// <returns></returns>
        public static List<T> FilterAbstractContext<T>(this IDbContext context, string search, string sortOrder, int start, int length,
            out int totalCount, Func<T, bool> dbSearch = null) where T : class, IBaseModel
        {
            var rh = dbSearch != null ? context.SetEntity<T>().Where(dbSearch).ToList() : context.SetEntity<T>().ToList();

            var result = rh.ToList<dynamic>().Where(p => FilterPredicate((p, search))).ToList();

            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();

            result = result.Order(sortOrder);

            return result.Adapt<List<T>>();
        }


        /// <summary>
        /// Custom filter for js data-table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static async Task<(List<T>, int)> Filter<T>(this DynamicObject context, string search, string sortOrder, int start, int length, Expression<Func<T, bool>> predicate = null) where T : class
        {
            var data = await context.GetAllWithInclude(predicate?.Compile());
            if (!data.IsSuccess) return default;
            var result = data.Result.ToList<dynamic>().Where(p => FilterPredicate((p, search))).ToList();

            var totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();

            result = result.Order(sortOrder);

            return (result.Adapt<List<T>>(), totalCount);
        }

        /// <summary>
        /// Filter dynamic entity
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="predicate"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static async Task<(List<object>, int)> Filter(this DynamicObject context, string entity, string search, string sortOrder, int start, int length, Expression<Func<object, bool>> predicate = null, IEnumerable<Filter> filters = null)
        {
            var data = await context.Service.Table(entity).GetAllWithInclude(predicate?.Compile(), filters: filters);
            if (!data.IsSuccess) return default;
            var result = data.Result.ToList<dynamic>().Where(p => FilterPredicate((p, search))).ToList();

            var totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();

            result = result.Order(sortOrder);

            return (result.Adapt<List<object>>(), totalCount);
        }

        /// <summary>
        /// Order list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        private static List<object> Order(this IList<object> list, string sortOrder)
        {
            if (!list.Any()) return list.ToList();
            try
            {
                var obj = list.FirstOrDefault();
                if (obj == null) return list.ToList();
                var props = obj.GetType().GetProperties();
                if (string.IsNullOrEmpty(sortOrder)) return list.ToList();
                var split = sortOrder.Split(' ');
                var toFind = split[0].ToLower();
                var isAsc = split.Length != 2;

                var property = props.FirstOrDefault(x => x.Name.ToLower().Equals(toFind));
                if (property == null) return list.ToList();
                {
                    list = isAsc ? list.OrderBy(x => x.GetType().GetProperty(property.Name).GetValue(x)).ToList()
                        : list.OrderByDescending(x => x.GetType().GetProperty(property.Name).GetValue(x)).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return list.ToList();
        }
    }
}
