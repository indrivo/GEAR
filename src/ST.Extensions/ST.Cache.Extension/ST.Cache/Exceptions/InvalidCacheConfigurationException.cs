using System;

namespace ST.Cache.Exceptions
{
    public class InvalidCacheConfigurationException : Exception
    {
        public InvalidCacheConfigurationException() : base("Cache credentials was not registered on appsettings")
        {

        }
    }
}
