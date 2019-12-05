using System;

namespace GR.Core.Exceptions
{
    public class DbContextNotRegisteredException : Exception
    {
        public DbContextNotRegisteredException(string message) : base(message)
        {

        }
    }
}
