using System;

namespace GR.Cache.Abstractions.Exceptions
{
    public class InvalidCacheConfigurationException : Exception
    {
        public InvalidCacheConfigurationException(string message) : base(message)
        {

        }

        public InvalidCacheConfigurationException() : base("Cache credentials was not registered on appsettings")
        {

        }
    }
}
