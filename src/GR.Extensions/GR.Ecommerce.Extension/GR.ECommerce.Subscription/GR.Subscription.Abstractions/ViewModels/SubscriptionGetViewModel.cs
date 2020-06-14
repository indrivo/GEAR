using System;
using System.Collections.Generic;

namespace GR.Subscriptions.Abstractions.ViewModels
{
    public class SubscriptionGetViewModel
    {
        /// <summary>
        /// Id of subscription
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Reference to customer
        /// </summary>
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
        /// is free
        /// </summary>
        public virtual bool IsFree { get; set; }

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

        /// <summary>
        /// Permissions
        /// </summary>
        public virtual IEnumerable<SubscriptionPermissionViewModel> Permissions { get; set; } = new List<SubscriptionPermissionViewModel>();
    }
}