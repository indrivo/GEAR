using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using ST.Cache.Abstractions;

namespace ST.Cache.Services
{
    public class CacheService : ICacheService
    {
        /// <summary>
        /// Inject distributed cache
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache"></param>
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Set new value
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual async Task<bool> Set<TObject>(string key, TObject obj) where TObject : class
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                await _cache.SetAsync(key, bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual async Task<TObject> Get<TObject>(string key) where TObject : class
        {
            var value = await _cache.GetAsync(key);
            if (value == null || value.Length == 0) return default;
            var str = Encoding.UTF8.GetString(value);
            if (typeof(TObject) == typeof(string)) return str as TObject;
            try
            {
                var data = JsonConvert.DeserializeObject<TObject>(str);
                return data;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<RedisKey> GetAllKeys()
        {
            var conn = new RedisConnection();
            return conn.GetAll();
        }

        /// <summary>
        /// Get all by pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public virtual IEnumerable<RedisKey> GetAllByPatternFilter(string pattern)
        {
            var conn = new RedisConnection();
            return conn.GetByPatternFilter(pattern);
        }

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
        public virtual bool IsConnected() => new RedisConnection().IsConnected();

        /// <summary>
        /// Flush all keys
        /// </summary>
        public virtual void FlushAll() => new RedisConnection().FlushAll();
    }
}
