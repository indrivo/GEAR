using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using GR.Cache.Abstractions;
using GR.Cache.Abstractions.Exceptions;
using GR.Core;
using StackExchange.Redis;

namespace GR.Cache.Services
{
    public class RedisConnection : IRedisConnection
    {
        /// <summary>
        /// Redis connection instance
        /// </summary>
        private static ConnectionMultiplexer RedisConnectionInstance { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        private readonly IOptions<RedisConnectionConfig> _redisConnectionOptions;

        /// <summary>
        /// Connection
        /// </summary>
        private ConnectionMultiplexer Connection => GetConnection();

        /// <summary>
        /// Prefix key
        /// </summary>
        private readonly string _preKey;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemOptions"></param>
        /// <param name="environment"></param>
        /// <param name="redisConnectionOptions"></param>
        public RedisConnection(IOptions<SystemConfig> systemOptions, IHostingEnvironment environment, IOptions<RedisConnectionConfig> redisConnectionOptions)
        {
            if (redisConnectionOptions.Value == null) throw new InvalidCacheConfigurationException();
            _preKey = $"{systemOptions.Value.MachineIdentifier}.{environment.EnvironmentName}@";
            _redisConnectionOptions = redisConnectionOptions;
        }

        /// <summary>
        /// Get conf options
        /// </summary>
        /// <returns></returns>
        private ConfigurationOptions GetConfigurationOptions()
        {
            var host = $"{_redisConnectionOptions.Value.Host}:{_redisConnectionOptions.Value.Port}";
            var options = ConfigurationOptions.Parse(host);
            return options;
        }

        /// <summary>
        /// Get instance
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnection()
        {
            if (RedisConnectionInstance != null)
            {
                if (RedisConnectionInstance.IsConnected)
                    return RedisConnectionInstance;
            }

            RedisConnectionInstance = CreateConnection();

            RedisConnectionInstance.ConnectionFailed += (sender, args) =>
                {
                    RedisConnectionInstance = CreateConnection();
                };

            return RedisConnectionInstance;
        }

        /// <summary>
        /// Create connection
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer CreateConnection()
        {
            var options = GetConfigurationOptions();
            return ConnectionMultiplexer.Connect(options);
        }

        /// <summary>
        /// Get end point
        /// </summary>
        /// <returns></returns>
        private EndPoint GetEndPoint()
        {
            var endPoint = Connection.GetEndPoints().FirstOrDefault();
            return endPoint;
        }

        /// <summary>
        /// Is connected
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return Connection.IsConnected;
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RedisKey> GetAll()
        {
            var endPoint = GetEndPoint();
            if (endPoint == null) return new Collection<RedisKey>();
            return Connection.GetServer(endPoint).Keys(pattern: $"{_preKey}*").ToList();
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
            return Connection.GetServer(endPoint).Keys(pattern: $"{_preKey}{filterPattern}").ToList();
        }

        /// <summary>
        /// Flush all
        /// </summary>
        public virtual void FlushAll()
        {
            var keys = GetAll().ToList();
            foreach (var key in keys)
            {
                Connection.GetDatabase().KeyDelete(key);
            }
        }
    }
}
