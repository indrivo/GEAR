using System;

namespace GR.Calendar.Abstractions.ExternalProviders.Helpers
{
    public class ExternalProviderConfig
    {
        /// <summary>
        /// Provider
        /// </summary>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Fa icon
        /// </summary>
        public virtual string FontAwesomeIcon { get; set; }

        /// <summary>
        /// Provider type
        /// </summary>
        public virtual Type ProviderType { get; set; }
    }
}