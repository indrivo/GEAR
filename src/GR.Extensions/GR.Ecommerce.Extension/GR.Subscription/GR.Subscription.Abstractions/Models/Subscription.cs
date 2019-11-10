using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GR.Core;
using GR.Core.Extensions;
using GR.Orders.Abstractions.Models;

namespace GR.Subscription.Abstractions.Models
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
        public int Valability { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; set; }

    }
}
