using System;

namespace ST.Calendar.Abstractions.ExternalProviders.Helpers
{
    public class ExternalProviderConfig
    {
        /// <summary>
        /// Provider
        /// </summary>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// Provider type
        /// </summary>
        public virtual Type ProviderType { get; set; }
    }
}