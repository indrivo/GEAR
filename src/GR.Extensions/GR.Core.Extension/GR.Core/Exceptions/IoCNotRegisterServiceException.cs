using System;

namespace GR.Core.Exceptions
{
    public class IoCNotRegisterServiceException : Exception
    {
        public IoCNotRegisterServiceException()
        {

        }

        public IoCNotRegisterServiceException(string message) : base(message)
        {

        }
    }
}
