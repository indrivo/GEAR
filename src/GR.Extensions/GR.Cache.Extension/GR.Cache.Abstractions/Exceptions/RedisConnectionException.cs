using System;

namespace GR.Cache.Abstractions.Exceptions
{
    public class RedisConnectionException : Exception
    {
        public RedisConnectionException(string exception) : base(exception)
        {

        }

        public RedisConnectionException()
        {

        }
    }
}
