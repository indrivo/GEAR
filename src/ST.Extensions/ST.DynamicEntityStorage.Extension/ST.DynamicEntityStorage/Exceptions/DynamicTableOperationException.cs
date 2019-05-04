using System;

namespace ST.DynamicEntityStorage.Exceptions
{
    public class DynamicTableOperationException : Exception
    {
        public DynamicTableOperationException(string message) : base(message)
        {

        }
    }
}
