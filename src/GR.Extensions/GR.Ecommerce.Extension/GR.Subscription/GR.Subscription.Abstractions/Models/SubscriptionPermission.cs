using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Subscriptions.Abstractions.Models
{
    public class SubscriptionPermission : BaseModel
    {
        /// <summary>
        /// Subscription
        /// </summary>
        public virtual Subscription Subscription { get; set; }

        [Required]
        public virtual Guid SubscriptionId { get; set; }

        /// <summary>
        /// Service name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        [Required]
        public virtual string Value { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public virtual ServiceValueType Type { get; set; } = ServiceValueType.String;
    }

    public enum ServiceValueType
    {
        String,
        Number,
        Array,
        External,
        Uuid,
        Entity,
        Money
    }
}
