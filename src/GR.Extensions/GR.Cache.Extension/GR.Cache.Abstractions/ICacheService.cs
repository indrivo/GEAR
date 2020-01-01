using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Cache.Abstractions.Models;

namespace GR.Cache.Abstractions
{
    public interface ICacheService
    {
        /// <summary>
        /// Set key in cache
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task<bool> SetAsync<TObject>(string key, TObject obj) where TObject : class;

        /// <summary>
        /// Get value key from cache
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TObject> GetAsync<TObject>(string key) where TObject : class;

        /// <summary>
        /// Remove key value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// Get all cache values
        /// </summary>
        /// <returns></returns>
        IEnumerable<CacheEntry> GetAllKeys();

        /// <summary>
        /// Get keys by pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<CacheEntry> GetAllByPatternFilter(string pattern);

        /// <summary>
        /// Check if provider is connected
        /// </summary>
        /// <returns></returns>
        bool IsConnected();


        /// <summary>
        /// Flush all keys
        /// </summary>
        void FlushAll();
    }
}
