using System;

namespace ST.Core.Exceptions
{
    internal class IoCNotRegisterServiceException : Exception
    {
        public IoCNotRegisterServiceException()
        {

        }

        public IoCNotRegisterServiceException(string message) : base(message)
        {

        }
    }
}
