using System;
using System.Linq;
using GR.Core.Helpers;
using GR.Core.Helpers.Async;
using Microsoft.EntityFrameworkCore;

namespace GR.Cache.Abstractions.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Get data from cache.
        /// For first time, all records are loaded into cache, after this, the query is executed from cache data until expiration.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="expireAfter"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> FromCache<TEntity>(this IQueryable<TEntity> query, TimeSpan expireAfter)
        {
            if (query == null) return null;
            var service = IoC.Resolve<ICacheService>();
            var source = service.GetOrSetWithExpireTimeAsync($"gear_data_{typeof(TEntity).FullName}", expireAfter,
                async () => await query.ToListAsync()).GetAwaiter().GetResult();

            return new AsyncEnumerable<TEntity>(source);
        }
    }
}