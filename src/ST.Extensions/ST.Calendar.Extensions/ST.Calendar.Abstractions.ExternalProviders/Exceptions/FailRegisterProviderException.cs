using System;

namespace ST.Calendar.Abstractions.ExternalProviders.Exceptions
{
    public class FailRegisterProviderException : Exception
    {
        public FailRegisterProviderException() : base("Invalid configuration on register calendar external provider")
        {

        }
    }
}
