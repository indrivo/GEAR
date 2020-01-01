using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Cache.Abstractions.Models;
using GR.Core.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace GR.Cache.Services
{
    public class InMemoryCacheService : ICacheService
    {
        #region Injectable

        /// <summary>
        /// Inject memory cache
        /// </summary>
        private readonly IMemoryCache _inMemoryCacheService;

        #endregion

        public InMemoryCacheService(IMemoryCache inMemoryCacheService)
        {
            _inMemoryCacheService = inMemoryCacheService;
        }

        public void FlushAll() { }

        /// <summary>
        /// Get value of key
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<TObject> GetAsync<TObject>(string key) where TObject : class
            => Task.Factory.StartNew(() => _inMemoryCacheService.Get<TObject>(key));

        public IEnumerable<CacheEntry> GetAllByPatternFilter(string pattern)
             => new List<CacheEntry>();

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CacheEntry> GetAllKeys()
        => _inMemoryCacheService.GetEntries()?.Select(x => new CacheEntry(x.Key.ToString(), x.Value)).ToList() ??
           new List<CacheEntry>();

        /// <summary>
        /// Is connected
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() => true;

        /// <summary>
        /// Remove key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
            => Task.Factory.StartNew(() => _inMemoryCacheService.Remove(key));

        /// <summary>
        /// Set new value to object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task<bool> SetAsync<TObject>(string key, TObject obj) where TObject : class
            => Task.Factory.StartNew(() =>
            {
                var ob = _inMemoryCacheService.Set(key, obj);
                return ob != null;
            });
    }
}
