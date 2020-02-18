using System;
using System.Linq;
using System.Linq.Expressions;

namespace GR.Core.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Func to expression
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Expression<Func<TObject, TResult>> ToExpression<TObject, TResult>(
            this Func<TObject, TResult> func)
            => x => func(x);

        /// <summary>
        /// Func to object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Expression<Func<TObject>> ToExpression<TObject>(this Func<TObject> func)
            => Expression.Lambda<Func<TObject>>(Expression.Call(func.Method));

        /// <summary>
        /// To func
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Func<TObject, TResult> ToFunc<TObject, TResult>(this Expression<Func<TObject, TResult>> expression)
            => expression.Compile();

        /// <summary>
        /// Get value or default
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<TObject, bool> GetValueOrDefault<TObject>(this Func<TObject, bool> func)
            => func ?? (x => true);

        /// <summary>
        /// Where clause with condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
            => condition ? query.Where(whereClause) : query;
    }
}
