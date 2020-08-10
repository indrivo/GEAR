using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Cache.Abstractions.Models;
using GR.Core.Helpers;

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

        /// <summary>
        /// Get provider name
        /// </summary>
        /// <returns></returns>
        string GetImplementationProviderName();

        /// <summary>
        /// Get or set request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<ResultModel<T>> GetOrSetResponseAsync<T>(string key, Func<Task<ResultModel<T>>> func) where T : class;

        /// <summary>
        /// Get or set value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> GetOrSetResponseAsync<T>(string key, Func<Task<T>> func) where T : class;

        /// <summary>
        /// Get or set in cache with expiration period
        /// Once time is left, the value is refreshed in cache
        /// This process is not automatic, is due on method call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="life">The time for value</param>
        /// <param name="func">Value resolver</param>
        /// <returns></returns>
        Task<T> GetOrSetWithExpireTimeAsync<T>(string key, TimeSpan life, Func<Task<T>> func)
            where T : class;
    }

    public interface ICacheService<T> : ICacheService
    {

    }
}