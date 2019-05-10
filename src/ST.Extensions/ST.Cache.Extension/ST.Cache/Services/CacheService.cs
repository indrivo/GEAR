using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using ST.Cache.Abstractions;
using ST.Cache.Exceptions;

namespace ST.Cache.Services
{
    public class CacheService : ICacheService
    {
        /// <summary>
        /// Inject distributed cache
        /// </summary>
        private readonly IDistributedCache _cache;


        /// <summary>
        /// Redis host
        /// </summary>
        private readonly string _redisHost;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="options"></param>
        public CacheService(IDistributedCache cache, IOptions<RedisConnectionConfig> options)
        {
            _cache = cache;
            if (options.Value == null) throw new InvalidCacheConfigurationException();
            _redisHost = $"{options.Value.Host}:{options.Value.Port}";
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
            var conn = new RedisConnection(_redisHost);
            return conn.GetAll();
        }

        /// <summary>
        /// Get all by pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public virtual IEnumerable<RedisKey> GetAllByPatternFilter(string pattern)
        {
            var conn = new RedisConnection(_redisHost);
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
        public virtual bool IsConnected() => new RedisConnection(_redisHost).IsConnected();

        /// <summary>
        /// Flush all keys
        /// </summary>
        public virtual void FlushAll() => new RedisConnection(_redisHost).FlushAll();
    }
}
