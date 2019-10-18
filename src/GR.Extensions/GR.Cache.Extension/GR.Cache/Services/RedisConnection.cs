using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using GR.Cache.Abstractions;
using GR.Core;
using StackExchange.Redis;
using InternalExceptions = GR.Cache.Exceptions;

namespace GR.Cache.Services
{
    public class RedisConnection : IRedisConnection
    {
        /// <summary>
        /// Redis connection
        /// </summary>
        private readonly ConnectionMultiplexer _redisConnection;

        private readonly string _preKey;

        public RedisConnection(IOptions<SystemConfig> systemOptions, IHostingEnvironment environment, IOptions<RedisConnectionConfig> redisConnectionOptions)
        {
            if (redisConnectionOptions.Value == null) throw new InternalExceptions.InvalidCacheConfigurationException();
            _preKey = $"{systemOptions.Value.MachineIdentifier}.{environment.EnvironmentName}@";
            //_host = "127.0.0.1:6379";
            var host = $"{redisConnectionOptions.Value.Host}:{redisConnectionOptions.Value.Port}";
            var options = ConfigurationOptions.Parse(host);
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
            return _redisConnection.GetServer(endPoint).Keys(pattern: $"{_preKey}*").ToList();
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
            return _redisConnection.GetServer(endPoint).Keys(pattern: $"{_preKey}{filterPattern}").ToList();
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
