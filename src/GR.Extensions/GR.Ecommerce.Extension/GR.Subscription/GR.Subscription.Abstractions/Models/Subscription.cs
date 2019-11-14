using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Orders.Abstractions.Models;

namespace GR.Subscriptions.Abstractions.Models
{
    public class Subscription: BaseModel
    {
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
        /// valability day
        /// </summary>
        public virtual int Valability { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// Is valid subscription
        /// </summary>
        public virtual bool IsValid => StartDate.AddDays(Valability) < DateTime.Now;

        /// <summary>
        /// Remaining days subscription
        /// </summary>
        public virtual int RemainingDays => IsValid ? (StartDate.AddDays(Valability) - DateTime.Now).Days : 0;
    }
}
