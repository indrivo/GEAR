using System;

namespace GR.Entities.Security.Abstractions.Exceptions
{
    public class EntitySecurityNotRegisteredServiceException : Exception
    {
        public EntitySecurityNotRegisteredServiceException()
        {

        }

        public EntitySecurityNotRegisteredServiceException(string message) : base(message)
        {

        }
    }
}
