namespace GR.Cache.Abstractions.Models
{
    public class CacheConfiguration
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
