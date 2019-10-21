using System;

namespace GR.Calendar.Abstractions.Models
{
    public class ExternalProviderToken
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Provider name
        /// </summary>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Attribute { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public virtual string Value { get; set; }
    }
}