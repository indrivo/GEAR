using System;

namespace GR.DynamicEntityStorage.Exceptions
{
    public class DynamicTableOperationException : Exception
    {
        public DynamicTableOperationException(string message) : base(message)
        {

        }
    }
}
