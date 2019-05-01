using System;

namespace ST.Cache.Exceptions
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
