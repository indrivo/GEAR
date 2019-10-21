using System;

namespace GR.Cache.Exceptions
{
    public class InvalidCacheConfigurationException : Exception
    {
        public InvalidCacheConfigurationException() : base("Cache credentials was not registered on appsettings")
        {

        }
    }
}
