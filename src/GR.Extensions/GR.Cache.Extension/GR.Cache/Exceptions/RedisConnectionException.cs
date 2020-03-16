using System;

namespace GR.Cache.Exceptions
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
