using System.Collections.Generic;
using StackExchange.Redis;

namespace GR.Cache.Abstractions
{
    public interface IRedisConnection
    {
        /// <summary>
        /// Check if is connected
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns></returns>
        IEnumerable<RedisKey> GetAll();
        /// <summary>
        /// Get list of keys by filters
        /// </summary>
        /// <param name="filterPattern"></param>
        /// <returns></returns>
        IEnumerable<RedisKey> GetByPatternFilter(string filterPattern);
        /// <summary>
        /// Flush all keys
        /// </summary>
        void FlushAll();
    }
}