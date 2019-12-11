using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Core;

namespace GR.Core.Extensions
{
    public static class QueryExtensions
    {
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var result = new PagedResult<T> {CurrentPage = page, PageSize = pageSize, RowCount = query.Count()};

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }

        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var result = new PagedResult<T> { CurrentPage = page, PageSize = pageSize, RowCount = query.Count() };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static IOrderedEnumerable<TSource> OrderByWithDirection<TSource, TKey>
        (this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            bool descending)
        {
            return descending ? source.OrderByDescending(keySelector)
                : source.OrderBy(keySelector);
        }

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
