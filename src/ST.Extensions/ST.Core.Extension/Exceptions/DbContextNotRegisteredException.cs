using System;

namespace ST.Core.Exceptions
{
    public class DbContextNotRegisteredException : Exception
    {
        public DbContextNotRegisteredException(string message) : base(message)
        {

        }
    }
}
