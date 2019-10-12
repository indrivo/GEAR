using System;

namespace ST.Calendar.Abstractions.ExternalProviders.Exceptions
{
    public class InvalidCalendarProviderNameException : Exception
    {
        public InvalidCalendarProviderNameException(string providerName) : base($"No provider registered with {providerName}")
        {

        }
    }
}
