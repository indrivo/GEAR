using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Orders.Abstractions.Models;

namespace GR.Subscriptions.Abstractions.Models
{
    public class Subscription : BaseModel
    {
        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Reference to customer
        /// </summary>
        [Required]
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Start date subscription
        /// </summary>
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// Availability days
        /// </summary>
        public virtual int Availability { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        public virtual Guid? OrderId { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// is free
        /// </summary>
        public virtual bool IsFree { get; set; }


        /// <summary>
        /// Services
        /// </summary>
        public virtual IEnumerable<SubscriptionPermission> SubscriptionPermissions { get; set; }

        /// <summary>
        /// Is valid subscription
        /// </summary>
        public virtual bool IsValid => ExpirationDate > DateTime.Now;

        /// <summary>
        /// Remaining days subscription
        /// </summary>
        public virtual int RemainingDays => IsValid ? (ExpirationDate - DateTime.Now).Days : 0;

        /// <summary>
        /// Expiration Date
        /// </summary>
        public virtual DateTime ExpirationDate => StartDate.AddDays(Availability);
    }
}
