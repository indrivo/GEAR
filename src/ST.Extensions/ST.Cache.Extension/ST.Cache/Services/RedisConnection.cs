using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using StackExchange.Redis;
using InternalExceptions = ST.Cache.Exceptions;

namespace ST.Cache.Services
{
    public class RedisConnection
    {
        /// <summary>
        /// Redis connection
        /// </summary>
        private readonly ConnectionMultiplexer _redisConnection;

        protected const string PreKey = ".ST.ISO.Data";

        public RedisConnection(string connection = "127.0.0.1:6379")
        {
            var options = ConfigurationOptions.Parse(connection);
            _redisConnection = ConnectionMultiplexer.Connect(options);
            if (!_redisConnection.IsConnected)
                throw new InternalExceptions.RedisConnectionException("Fail to connect with redis server");
        }

        /// <summary>
        /// Get end point
        /// </summary>
        /// <returns></returns>
        private EndPoint GetEndPoint()
        {
            var endPoint = _redisConnection.GetEndPoints().FirstOrDefault();
            return endPoint;
        }

        /// <summary>
        /// Is connected
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _redisConnection.IsConnected;
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RedisKey> GetAll()
        {
            var endPoint = GetEndPoint();
            if (endPoint == null) return new Collection<RedisKey>();
            return _redisConnection.GetServer(endPoint).Keys(pattern: $"{PreKey}*").ToList();
        }

        /// <summary>
        /// Get all keys by pattern
        /// </summary>
        /// <param name="filterPattern"></param>
        /// <returns></returns>
        public IEnumerable<RedisKey> GetByPatternFilter(string filterPattern)
        {
            var endPoint = GetEndPoint();
            if (endPoint == null) return new Collection<RedisKey>();
            return _redisConnection.GetServer(endPoint).Keys(pattern: $"{PreKey}{filterPattern}").ToList();
        }

        /// <summary>
        /// Flush all
        /// </summary>
        public virtual void FlushAll()
        {
            var keys = GetAll().ToList();
            foreach (var key in keys)
            {
                _redisConnection.GetDatabase().KeyDelete(key);
            }
        }
    }
}
