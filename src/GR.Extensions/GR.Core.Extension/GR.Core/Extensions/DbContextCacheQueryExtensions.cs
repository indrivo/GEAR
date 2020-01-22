using System;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GR.Core.Extensions
{
    public static class DbContextCacheQueryExtensions
    {
        //TODO: Write methods for delete, update, get all
        private static IMemoryCache ProtectedPropCacheService { get; set; }
        private static IMemoryCache CacheService => ProtectedPropCacheService ?? (ProtectedPropCacheService = IoC.Resolve<IMemoryCache>());

        /// <summary>
        /// Find by id on memory cache
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<TEntity> FindByIdOnCacheAsync<TEntity>(this DbSet<TEntity> self, Guid? id)
            where TEntity : class, IBaseModel, IBase<Guid>
        {
            if (!id.HasValue) return default;
            var key = GenerateKey<TEntity>(id.Value);
            var obj = CacheService.Get<TEntity>(key);
            if (obj != null) return obj;
            var dbObject = await self.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (dbObject == null) return default;
            CacheService.Set(key, dbObject);
            return dbObject;
        }


        #region Helpers

        /// <summary>
        /// Generate key
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string GenerateKey<TEntity>(Guid id)
            where TEntity : class, IBaseModel
        {
            var entityName = typeof(TEntity).Name;
            return $"temp_entity_{entityName}_bind_on_{id}";
        }

        #endregion
    }
}
