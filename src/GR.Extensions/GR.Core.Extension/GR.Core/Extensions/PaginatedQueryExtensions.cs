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
        /// <returns></returns>
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
            DTParameters parameters)
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
                var propName = split[0].ToLower().FirstCharToUpper();
                var isAsc = split.Length != 2;
                query = query.OrderByWithDirection(x => x.GetPropertyValue(propName), !isAsc);
            }
            else
            {
                query = query.OrderByWithDirection(x => x.GetPropertyValue(nameof(BaseModel.Created)), true);
            }

            result.RowCount = await query.CountAsync();
            var pageCount = (double)result.RowCount / parameters.Length;
            result.PageCount = (int)Math.Ceiling(pageCount);

            result.Result = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Get paged response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<DTResult<T>> GetPagedAsDtResultAsync<T>(this IQueryable<T> query,
            DTParameters parameters)
            where T : class
        {
            var paged = await query.GetPagedAsync(parameters);
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
    }
}
