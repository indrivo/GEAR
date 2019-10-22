namespace GR.Cache.Abstractions
{
    public sealed class RedisConnectionConfig
    {
        /// <summary>
        /// Host of redis
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port what is opened for redis connection
        /// </summary>
        public string Port { get; set; }
    }
}
