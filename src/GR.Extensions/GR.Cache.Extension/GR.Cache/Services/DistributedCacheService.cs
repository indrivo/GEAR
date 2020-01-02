using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using GR.Cache.Abstractions;
using GR.Cache.Abstractions.Models;
using GR.Core.Extensions;

namespace GR.Cache.Services
{
    public class DistributedCacheService : ICacheService
    {
        /// <summary>
        /// Inject distributed cache
        /// </summary>
        private readonly IDistributedCache _cache;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Inject redis manager
        /// </summary>
        private readonly IRedisConnection _redisConnection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="redisConnection"></param>
        public DistributedCacheService(IDistributedCache cache, IRedisConnection redisConnection)
        {
            _cache = cache;
            _redisConnection = redisConnection;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };
        }

        /// <summary>
        /// Set new value
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<bool> SetAsync<TObject>(string key, TObject obj) where TObject : class
        {
            try
            {
                await _cache.SetAsync(key, JsonConvert.SerializeObject(obj, _jsonSerializerSettings).ToBytes());
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TObject> GetAsync<TObject>(string key) where TObject : class
        {
            var value = await _cache.GetAsync(key);
            if (value == null || value.Length == 0) return default;
            var str = Encoding.UTF8.GetString(value);
            if (typeof(TObject) == typeof(string)) return str as TObject;
            try
            {
                var data = JsonConvert.DeserializeObject<TObject>(str, _jsonSerializerSettings);
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return default;
            }
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CacheEntry> GetAllKeys()
            => Map(_redisConnection.GetAll());

        /// <summary>
        /// Get all by pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public virtual IEnumerable<CacheEntry> GetAllByPatternFilter(string pattern)
            => Map(_redisConnection.GetByPatternFilter(pattern));

        /// <summary>
        /// Remove key from cache service
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

        /// <summary>
        /// Is redis connected
        /// </summary>
        /// <returns></returns>
        public virtual bool IsConnected() => _redisConnection.IsConnected();

        /// <summary>
        /// Flush all keys
        /// </summary>
        public virtual void FlushAll() => _redisConnection.FlushAll();


        #region Helpers

        /// <summary>
        /// Map redis keys to cache entry
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private static IEnumerable<CacheEntry> Map(IEnumerable<RedisKey> keys)
            => keys.Select(x => x.ToString()).Select(x => new CacheEntry(x, ""));

        #endregion
    }
}
