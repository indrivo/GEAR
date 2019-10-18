using System;

namespace GR.Cache.Exceptions
{
    internal class RedisConnectionException : Exception
    {
        public RedisConnectionException(string exception) : base(exception)
        {

        }

        public RedisConnectionException()
        {

        }
    }
}
