using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using GR.Core.Helpers.Pagination;

namespace GR.Core.Extensions
{
    public static class PaginatedQueryExtensions
    {
        /// <summary>
        /// Get paged result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var result = new PagedResult<T> { CurrentPage = page, PageSize = pageSize, RowCount = query.Count() };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Result = await query.Skip(skip).Take(pageSize).ToListAsync();
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// Get paged response with filters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TPageRequest"></typeparam>
        /// <param name="query"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T, TPageRequest>(this IQueryable<T> query, TPageRequest request)
            where T : class
            where TPageRequest : PageRequest
        {
            if (query == null) throw new NullReferenceException("Invalid query");
            if (request == null) throw new NullReferenceException("Request can't be null");

            if (request.PageRequestFilters.Any())
            {
                query = query.FilterSourceByFilters(request.PageRequestFilters);
            }

            if (!request.RegexExpression.IsNullOrEmpty())
            {
                query = query.FilterSourceByRegEx(request.RegexExpression);
            }

            if (!request.GSearch.IsNullOrEmpty())
            {
                query = query.FilterSourceByTextExpression(request.GSearch);
            }

            if (!request.Attribute.IsNullOrEmpty())
            {
                query = query.OrderByWithDirection(x => x.GetPropertyValue(request.Attribute), request.Descending);
            }

            var result = new PagedResult<T>
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                RowCount = await query.CountAsync()
            };

            var pageCount = (double)result.RowCount / request.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (request.Page - 1) * request.PageSize;
            result.Result = await query.Skip(skip).Take(request.PageSize).ToListAsync();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get paged result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var count = query.Count();
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = count,
                TotalNonFiltered = count
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Result = query.Skip(skip).Take(pageSize).ToList();
            result.IsSuccess = true;
            return result;
        }

        #region Paged for DTParameters

        /// <summary>
        /// Get paged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
            DTParameters parameters, IEnumerable<DTColumnMap> mappings = null)
            where T : class
        {
            if (query == null) throw new NullReferenceException("Invalid query");

            var result = new PagedResult<T>
            {
                CurrentPage = parameters.Draw,
                PageSize = parameters.Length,
                TotalNonFiltered = await query.CountAsync()
            };

            if (parameters.Search != null && !parameters.Search.Value.IsNullOrEmpty())
            {
                query = parameters.Search.Regex
                    ? query.FilterSourceByRegEx(parameters.Search.Value)
                    : query.FilterSourceByTextExpression(parameters.Search.Value);
            }

            if (!parameters.SortOrder.IsNullOrEmpty())
            {
                var split = parameters.SortOrder.Split(' ');
                var propName = split[0].FirstCharToUpper();
                var isAsc = split.Length != 2;

                if (mappings != null)
                {
                    var hasMap = mappings.FirstOrDefault(x => x.FromColumn == propName);
                    if (hasMap == null)
                    {
                        query = query.OrderByWithDirection(propName, !isAsc);
                    }
                    else
                    {
                        query = query.OrderByWithDirection(hasMap.ToColumn, !isAsc);
                    }
                }
                else
                {
                    query = query.OrderByWithDirection(propName, !isAsc);
                }
            }
            else
            {
                query = query.OrderByWithDirection(nameof(BaseModel.Created), true);
            }

            result.RowCount = await query.CountAsync();
            var pageCount = (double)result.RowCount / parameters.Length;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.Result = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get paged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
            DTParameters parameters, IEnumerable<DTColumnMap> mappings = null)
            where T : class
        {
            if (query == null) throw new NullReferenceException("Invalid query");

            var result = new PagedResult<T>
            {
                CurrentPage = parameters.Draw,
                PageSize = parameters.Length,
                TotalNonFiltered = query.Count()
            };

            if (parameters.Search != null && !parameters.Search.Value.IsNullOrEmpty())
            {
                query = parameters.Search.Regex
                    ? query.FilterSourceByRegEx(parameters.Search.Value)
                    : query.FilterSourceByTextExpression(parameters.Search.Value);
            }

            if (!parameters.SortOrder.IsNullOrEmpty())
            {
                var split = parameters.SortOrder.Split(' ');
                var propName = split[0].FirstCharToUpper();
                var isAsc = split.Length != 2;

                if (mappings != null)
                {
                    var hasMap = mappings.FirstOrDefault(x => x.FromColumn == propName);
                    if (hasMap == null)
                    {
                        query = query.OrderByWithDirection(propName, !isAsc);
                    }
                    else
                    {
                        query = query.OrderByWithDirection(hasMap.ToColumn, !isAsc);
                    }
                }
                else
                {
                    query = query.OrderByWithDirection(propName, !isAsc);
                }
            }
            else
            {
                query = query.OrderByWithDirection(nameof(BaseModel.Created), true);
            }

            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / parameters.Length;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.Result = query.Skip(parameters.Start).Take(parameters.Length).ToList();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get paged response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public static async Task<DTResult<T>> GetPagedAsDtResultAsync<T>(this IQueryable<T> query, DTParameters parameters, IEnumerable<DTColumnMap> mappings = null)
            where T : class
        {
            var paged = await query.GetPagedAsync(parameters, mappings);
            return new DTResult<T>
            {
                Draw = paged.CurrentPage,
                Data = paged.Result.ToList(),
                RecordsFiltered = paged.RowCount,
                RecordsTotal = paged.TotalNonFiltered
            };
        }

        /// <summary>
        /// Get paged response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public static DTResult<T> GetPagedAsDtResult<T>(this IQueryable<T> query, DTParameters parameters, IEnumerable<DTColumnMap> mappings = null)
            where T : class
        {
            var paged = query.GetPaged(parameters, mappings);
            return new DTResult<T>
            {
                Draw = paged.CurrentPage,
                Data = paged.Result.ToList(),
                RecordsFiltered = paged.RowCount,
                RecordsTotal = paged.TotalNonFiltered
            };
        }

        #endregion


        /// <summary>
        /// Order source items with direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
        (this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);
        }


        /// <summary>
        /// Order source items with direction
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TSource> OrderByWithDirection<TSource, TKey>
        (this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);
        }

        /// <summary>
        /// Order by descending by property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertyName"></param>
        /// <param name="descending"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByWithDirection<T>(this IQueryable<T> query, string propertyName, bool descending, IComparer<object> comparer = null)
        {
            return descending ? query.OrderByDescending(propertyName, comparer) : query.OrderBy(propertyName, comparer);
        }

        /// <summary>
        /// Order by property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertyName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        /// <summary>
        /// Order by descending
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertyName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        /// <summary>
        /// The order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertyName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
        }

        /// <summary>
        /// Then by descending
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="propertyName"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
        }

        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// </summary>
        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
                IComparer<object> comparer = null)
        {
            if (!propertyName.Contains(".")) //check if property exist in first level
            {
                var property = typeof(T).GetProperty(propertyName);
                if (propertyName == nameof(BaseModel.Created) && property == null)
                {
                    return query.OrderBy(x => 0);
                }

                if (property == null)
                {
                    return query.CallOrderedQueryable(methodName, nameof(BaseModel.Created), comparer);
                }
            }

            var param = Expression.Parameter(typeof(T), "x");
            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }
    }
}
